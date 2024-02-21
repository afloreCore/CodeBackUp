using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GrwPinPadNet
{
    [Guid("538714c3-28da-49ae-ae62-82b7307a4519"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPosNet
    {
        double Importe { get; }
        string Factura { get; }
        string CodOperador { get; set; }
        string NroCupon { get; }
        EmpTarjeta CodTarjeta { get; }
        string CodAutorizacion { get; }
        short Cuotas { set;  get; }
        short PlandDeCuotas { set; }
        string IDTransaccion { get; }
        string CUIT { get; }
        string CUITISV { set; }
        string IPSITEF { get; }
        string COMERCIO_ID { get; }
        string TERMINAL_ID { get; }
        string ErrorLogCode { get; }
        string ErrorLogDescrp { get; }
        /// <summary>
        /// Tiempo de espera en (segundos) para cada operación, si se execede la operación será abortada
        /// </summary>
        double TimeOutSec { set; }
        /// <summary>
        /// El formato debe ser-> AAA.BBB.CCC.DDD:EEEE
        /// Donde AAA.BBB.CCC.DDD en la dirección IP, y EEEE el puerto.
        /// También puede recibir el nombre Host del terminal Clover
        /// </summary>
        string IPClover { get; }
        string CodAtorizaTarjeta { get; }
        string PrimerosDigitos { get; }
        string UltimosDigitos { get; }
        string PaymentID { set; get; }
        string OrderID { set; get; }
        string Lote { set; get; }
        /// <summary>
        /// Retorna string concatenado una lista de la clase TransPending.
        /// Asumiento que ListDelimiter = "#" y FieldsDelimiter=";"
        /// Con el siguiente formato: "Date=yyyymmdd;Comprobante=xxxxx;Import=00000,00#
        /// </summary>
        /// <param name="ListDelimiter">Separador de elementos de la lista</param>
        /// <param name="FieldsDelimiter">Separador de elementos dentro de la lista</param>
        /// <returns>String</returns>
        string GetListPending(string ListDelimiter, string FieldsDelimiter);
        /// <summary>
        /// Recibe una cadena de string de los comprobantes pendientes a cancelar.
        /// Asumiento que ListDelimiter = "#" y FieldsDelimiter=";"
        /// Con el siguiente formato: "Date=yyyymmdd;Comprobante=xxxxx;Import=00000,00#
        /// </summary>
        /// <param name="ListDelimiter">Separador de elementos de la lista</param>
        /// <param name="FieldsDelimiter">Separador de elementos dentro de la lista</param>
        void CancelListPending(string ListPending, string ListDelimiter, string FieldsDelimiter);
        /// <summary>
        /// Separa de la lista el último comprobante pendiente, sí existe
        /// Luego se obtienen los campos en las propertys: DateLastTrPending, CompLastTrPending, ImportLastTrPending
        /// </summary>
        string GetLastTrPending(string ListDelimiter, string FieldsDelimiter);
        string DateLastTrPending();
        string HourLastTrPending();
        string CompLastTrPending();
        string ImportLastTrPending();
        /// <summary>
        /// Carga una colección con las claves del archivo .dat
        /// </summary>
        /// <param name="ListDelimiter">string separador entre cada KeyValuePair</param>
        /// <param name="NexoDelimiter">string de igualación entre Key y Value</param>
        /// <returns></returns>
        string GetKeysValuesFromDataFile(string ListDelimiter = "", string NexoDelimiter = "");
      
        /// <summary>
        /// Obtiene el valor de la clave solicitada desde el archivo .dat.
        /// Antes del primer llamado es necesario invocar al método: GetKeysValuesFromDataFile
        /// </summary>
        /// <param name="Key">Clave a buscar: FISVID, TR, CUPON</param>
        /// <param name="ListDelimiter">String separador entre cada KeyValuePair</param>
        /// <param name="NexoDelimiter">string de igualación entre Key y Value</param>
        /// <returns>String correspondiente al valora la clave Key</returns>
        string GetValueFromDataFile(string Key, string ListDelimiter = "", string NexoDelimiter = "");
        bool Inicializado { get; set; }
        string GetStringKeyValue { get; }
        void StringKeyPairValue(string Cadena, string SplitChar);
        /// <summary>
        /// Código de autorización para operaciones gerenciales. Por ejemplo: devoluciones.
        /// En el caso de Clover, se debe enviar la clave Admin del dispositivo
        /// </summary>
        string CodSupervisor { set; }
        /// <summary>
        ///Para Clover: Recibe/Devuelve un string de la forma clave1=valor1|clave2=valor2. Para luego descomponerlo al realizar la devolución.
        /// ORDERID
        /// PAYMENTID
        /// NSU
        /// Para PinPad Recibe/Devuelve el código interno del comprobante a devolver
        /// </summary>
        string CodTransSitef { get; set; }
        /// <summary>
        /// Indicar a la librería si se debe imprimir los cupones
        /// </summary>
        bool Imprimir { set; }
        bool FinishAuto { set; }
        /// <summary>
        /// Tiempo en Milisegundos (ms) para emitir la copia del cupón. Tiempo default = 5000ms (5seg).
        /// </summary>
        int TiempoEsperaCopia { set; get; }
        /// <summary>
        /// Devuelve un valor mayor a cero si la operación en curso no se confirmó
        /// </summary>
        int CantidadPendientes { get; }
        string Fecha { get; }
        /// <summary>
        /// Hora de la transacción. Si es una devolución, debe ser la misma que la transacción original
        /// (HH:MM:SS)
        /// </summary>
        string HHMMSS { set; get; }
        string AssemblyDirectory { get; }
        string SetIniFile { set; }
        bool DebugPrint { set; }
        /// <summary>
        /// Devolución o alguna operación general
        /// </summary>
            TipoOperacion TipoOperacion { get; }
        /// <summary>
        /// Indica el tipo de forma de pago que se utilizará: 0: Cualquier tipo de operación (se solicitará la intervención del operador),
        /// 2(Débito), 3(Crédito): para poder automatizar lsa operaciones, si se agregan [Parámetros de Restricciones] en la función de inicio de la transacción
        /// </summary>
        TipoFuncion Funcion { get; }
        /// <summary>
        /// Combinado con el TipoFunción, se configura un subgrupo para el tipo de operación a realizar(Tarjetas de crédito, Débito, Reimpresión, etc...)
        /// </summary>
        TipoFormaPago FormaDePago { get; }
        /// <summary>
        /// Tipo de tarjeta, cuando la operación es del tipo monetaria.
        /// Las tarjetas pueden ser físicas o de ingreso manual
        /// </summary>
        TipoTarjeta TipoDeTarjeta { get; }
        /// <summary>
        /// Tipo de dispositivo que se comunicará con el PDV y Sitef
        /// </summary>
        TipoPinPad TipoDePinPad { get; }
        /// <summary>
        /// Código de la empresa de la tarjeta
        ///Otro = 0, Visa = 1, MasterCard = 2, Diners = 3, Amex = 4
        /// </summary>
        EmpTarjeta CodempTarjeta { set; }

        IReturnValue ReturnValue{set; get; }
        void WriteIniFile(string Section, string Key, string Value);
        string CodEmparejamiento { get; }
        void ConfigSiteSitef(string IPSitef, string TerminalID, string Comercio, string Cuit, string CUITisv = "", string args = "");
        void ConfigTransaction(TipoFuncion TipoDeFuncion, TipoOperacion TipoDeOperacion = TipoOperacion.General, TipoPinPad Dispositivo = TipoPinPad.PinPad, int PuerCOM = 0, string IPClover = "");
        bool Begin(string NroComprobante, double Monto, TipoFormaPago TipoPago, TipoTarjeta TipoTarjeta, EmpTarjeta CodTarjeta, string FechaComprobante = "", short NroDeCuotas = 0, string Usuario = "", string CompNSU = "");

        void Dispose();
    }
}
