using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Globalization;

namespace GrwPinPadNet
{
    /// <summary>
    ///Obligatorio. Indica la operatoria: Si es una Devolución o alguna operación general
    /// </summary>
    [GuidAttribute("2b6fff77-e3b6-4da6-99dd-71e75b88c48e"), ComVisible(true), DefaultValue(TipoOperacion.Ninguno)]
    public enum TipoOperacion:int
    { Ninguno=0, General, Devolucion}
    /// <summary>
    /// Indica el tipo de Forma de pago de la transacción-> 0: Cualquier tipo de operación (se solicitará la intervención del operador),
    /// 2(Débito), 3(Crédito): para poder automatizar lsa operaciones, si se agregan [Parámetros de Restricciones] en la función de inicio de la transacción
    /// </summary>
   
    [GuidAttribute("7f527b04-fe1f-436f-8436-2f09693014cf"), ComVisible(true), DefaultValue(TipoFuncion.PGeneral)]
    public enum TipoFuncion
    {PGeneral=0, Cheque, Debito, Credito, Gerencial=110, MenuReimpresion=112, ReimpresionConID=113, ReimpresionUltimo=114, PendientesTodos=130, PendienteID=131,  DevolucionGral=200, DevolucionTCDIG=201, DevolucionPreAuto=202, DevolucionTCMAG=210, DevolucionTDMAG, CargaTablaPinPad =770, ForzarCargaTablaPinPad=772, DevolucionClover=1902, ConfigClover=1904}
    /// <summary>
    /// 
    /// </summary>
    [GuidAttribute("4cb97091-9a15-456a-996b-48a471b4e3e5"), ComVisible(true), DefaultValue(TipoPinPad.Ninguno)]
    public enum TipoPinPad:int
    { Ninguno=0, PinPad, Clover=5}
    /// <summary>
    /// Combinado con el TipoFunción, se configura un subgrupo para el tipo de medio de pago que se utilizará 
    /// Tarjetas de crédito, Débito, Reimpresión, etc...
    /// </summary>
    [GuidAttribute("30c3cf9b-0937-483b-a33c-1dd58a8546ff"), ComVisible(true), DefaultValue(TipoFormaPago.Niguno)]
    public enum TipoFormaPago:int
    /*
    Tarjeta de débito (todas las combinaciones) 15
    Tarjeta de débito "en efectivo" (sin cuotas) 16
    Tarjeta de débito con cuotas 18
    Tarjeta de crédito "en efectivo" con intereses 24
    Tarjeta de crédito (todas las combinaciones) 25
    Tarjeta de crédito "en efectivo" 26
    Tarjeta de crédito con reembolsos financiados por el comerciante 27
    Tarjeta de crédito con reembolsos financiados por el emisor de la tarjeta 28
    Tarjeta de crédito ingresada con clave 29
    Tarjeta de crédito magnética 30
    Cancelación de una transacción con tarjeta de crédito o débito 40
    Clave de débito ingresada 42
    Débito magnético 43
    Reimprimir 56
    Reimpresión del último recibo 57
    Reimpresión específica de recibos 58
    Compra y Retiro 4151
    */
    {Niguno = 0, TDebitoTodas=15, TDebitoEFT=16, TDebitoCuotas=18, TCreditoEFTCI=24, TcreditoTodas, TCreditoEFT, 
        TCreditoFinaciaComercio, TCreditoFinanciaTarjeta , CancelCTARJ=40,  ReimprimirConCuotas=50,
        Reimprimir=56, ReimprimirUltimo=57, ReimprimirConID=58, ReimpresionTodas=72, ReimpresionOtros= 3675, CompraRetiro =4151}

    /// <summary>
    /// Tipo de tarjeta, cuando la operación es del tipo monetaria.
    /// Las tarjetas pueden ser físicas o de ingreso manual.
    /// </summary>
    [GuidAttribute("6c599600-cbf4-4350-a5d4-00d5b937495c"), ComVisible(true), DefaultValue(TipoTarjeta.Niguno)]
    public enum TipoTarjeta : int
    { Niguno = 0, TCreditoDIG =29, TCreditoMAG=30, ClaveDebitoDIG=42, TDebitoMAG=43 }

    [GuidAttribute("d3462fef-277f-471f-88e9-82311fa084a5"), ComVisible(true), DefaultValue(EmpTarjeta.Otro)]
    public enum EmpTarjeta : int
    /*  00000 Otro, no definido
        00001 Visa
        00002 Mastercard
        00003 Diners
        00004 American Express
        00005
    */
    { Otro = 0, Visa, MasterCard, Diners, Amex }

        /// <summary>
        /// ja_JP: yyyyMMdd, en_US: MMddyyyy, fr_FR: ddMMyyyy
        /// </summary>
        [GuidAttribute("5524ca06-0639-4c23-8b3e-7503f9a3eadb"), ComVisible(true), DefaultValue(TypeDate.ja_JP)]
    public enum TypeDate
    { ja_JP, en_US, fr_FR}

    [ComVisible(false)]
    public enum TipoInicializacion
    { Construct, ConfigurationSitef, ConfigurationIni, Transaction, Finish }

    [GuidAttribute("eff09a4e-cc9f-45e0-852d-82804ae3ee53"), ComVisible(true)]
    public static class FormatDate
    {
        #region String Format
        static string en_US { get => "MMddyyyy"; }
        static string fr_FR { get => "ddMMyyyy"; }
        static string ja_JP { get => "yyyyMMdd"; }

        #endregion
        #region Culture Format
        static string en_US_Culture { get => "en-US"; }
        static string fr_FR_Culture { get => "fr-FR"; }
        static string ja_JP_Culture { get => "ja-JP"; }

        #endregion
        public static string StringFormat(TypeDate td)
        {
            string _format = "";
            switch(td)
            {
                case TypeDate.ja_JP:
                    _format = ja_JP;
                    break;
                case TypeDate.en_US:
                    _format = en_US;
                    break;
                case TypeDate.fr_FR:
                    _format = fr_FR;
                    break;
            }

            return _format;
        }
        public static string StringFormat(CultureInfo culture)
        {

            return culture.DateTimeFormat.ToString();
        }
        public static string StringValideDate(TypeDate td)
        {
            string _format = "";
            switch (td)
            {
                case TypeDate.ja_JP:
                    _format = "yyyy-MM-dd";
                    break;
                case TypeDate.en_US:
                    _format = "MM-dd-yyyy";
                    break;
                case TypeDate.fr_FR:
                    _format = "dd-MM-yyyy";
                    break;
            }

            return _format;
        }
        public static CultureInfo CultureFormat(TypeDate td)
        {
            string _format = "";
            switch (td)
            {
                case TypeDate.ja_JP:
                    _format = ja_JP_Culture;
                    break;
                case TypeDate.en_US:
                    _format = en_US_Culture;
                    break;
                case TypeDate.fr_FR:
                    _format = fr_FR_Culture;
                    break;
            }

            return new CultureInfo(_format); 
        }
    }
   

}
    
