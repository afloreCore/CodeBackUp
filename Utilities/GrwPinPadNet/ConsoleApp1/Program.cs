using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;

namespace GrwPinPadNet
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime _fchmov;
            DateTime hoy = DateTime.Now.Date;
            string date = "06/09/2022";
            if (DateTime.TryParse(date, FormatDate.CultureFormat(TypeDate.fr_FR), DateTimeStyles.None, out _fchmov))
            {
                var dif = hoy.CompareTo(_fchmov);
            }
            // var tru = Utilities.DateString(date, TipoFuncion.DevolucionGral, out _);
            Main2Program m = new Main2Program();
            m.main();
            Console.ReadLine();
            //string listdelimiter = ";";
            //string nexodelimiter = "=";
            //PinPad oPinPad = new PinPad();
            ////var enc = Utilities.Encrypt("Hola mundo 1234!@");
            ////var dec  = Utilities.Decrypt(enc);
            //Dictionary<string, string> dicdatafile = new Dictionary<string, string>();
            //string file = @"C:\posnet.dat";
            //dicdatafile.Add("PK", "1234");
            //dicdatafile.Add("TR", "N2gdfdfd223");
            ////dicdatafile.Add("CUPON", "00032223112");
            ////dicdatafile.Add("FECHA", "20220515");
            //Utilities.WriteDataFile(file, listdelimiter, nexodelimiter, dicdatafile);
            //Utilities.GetKeysValuesFromDataFile(file, listdelimiter, nexodelimiter);
            //var pk = Utilities.GetValueFromDataFile(listdelimiter, nexodelimiter, "PK");
            //var tr = Utilities.GetValueFromDataFile(listdelimiter, nexodelimiter,"TR");
           
            //Console.ReadLine();
            /*
            var nose="";
            Dictionary<string, string> d = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in d)
            {
                nose = d[""];
            }
                string cad = "Date=20220125;Comprobante=250010;Import=30,22#Date=20220125;Comprobante=250016;Import=100# #";
                Utilities.StringToListClass(cad, "#",";");
            
            string s = ";";
            char c = s[0];
            List<TransPending> l = new List<TransPending>();
            TransPending t = new TransPending();
            t.Date = DateTime.Now.ToShortDateString();
            t.Import = "130,32";
            t.Comprobante = "123";
            l.Add(t);
            t = new TransPending();

            t.Date = DateTime.Now.ToShortDateString();
            t.Import = "50,32";
            t.Comprobante = "456";
            l.Add(t);
            string separador = ";";
            Func<TransPending, string> splistr = str => string.Concat(str.Date, separador, str.Comprobante, separador, str.Import);
            var ls = Utilities.ConcatStrList(l, "#", separador);
            */
            //foreach (var o in l)
            //{
            //    //note that checking the type for each object enables you to have heterogenous lists if you want
            //    var objectType = o.GetType();
            //    foreach (var v in objectType.GetProperties())
            //    {
            //        var propertyName = v.Name;
            //        Console.WriteLine( v.GetValue(o).ToString());



            //string str = "123456 cupon: 000012345678";
            //int index = str.ToUpper().LastIndexOf("CUPON:");
            //int index2 = str.IndexOf(":", index);
            //string str2 = str.Substring(index2+1, 13).TrimStart();
            //Int64 nrocupon;
            //if (!Int64.TryParse(str2, out nrocupon))
            //    return;
            /*prueba split
            string values = "k1=1|k2=2|k3=3|kn=n|";
            string s;
            Dictionary<string, string> d = values.Split('|')
                                            .Select(x => x.Split('=')).Where(y => !string.IsNullOrWhiteSpace(y[0]))
                                            .ToDictionary(x => x[0], x => x[1]);
                                          
            
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ",";
            string valor = "20.1";
            valor = string.Format("{0:#.00}", valor);
            if (!string.IsNullOrWhiteSpace(valor) && Convert.ToDecimal(valor) > 0 && !valor.Contains(","))
            {
                decimal d = Convert.ToDecimal(valor);
                string r = d.ToString("#.00").Replace('.',',');
            
            }
            */
            //string _ultimomensaje = "EN PROCESO(10)";
            //string mensagem = "EN PROCESO(35)";
            //if (_ultimomensaje.PadRight(10, ' ').Substring(0,10) != mensagem.PadRight(10, ' ').Substring(0, 10))
            //    Console.WriteLine("mensagem: {0}, _ultimo: {1}", mensagem.PadRight(10, ' ').Substring(0, 10), _ultimomensaje.PadRight(10, ' ').Substring(0, 10));

            PinPad p = new PinPad();
            TipoPinPad _tp = TipoPinPad.PinPad;
            TipoFuncion tf = TipoFuncion.PGeneral;
            TipoFuncion tfdev = TipoFuncion.DevolucionGral;
            TipoFormaPago tfpago = TipoFormaPago.TcreditoTodas;
            TipoTarjeta tptj = TipoTarjeta.Niguno;
            EmpTarjeta emptj = EmpTarjeta.Otro;
            //{PGeneral=0, Cheque, Debito, Credito, Gerencial=110, MenuReimpresion=112, ReimpresionConID=113, ReimpresionUltimo=114,  DevolucionGral=200, DevolucionTCDIG=201, DevolucionPreAuto=202, DevolucionTCMAG=210, DevolucionTDMAG, CargaTablaPinPad =770, ForzarCargaTablaPinPad=772, ConfigClover=1904}
            TipoFuncion tpfuncion;
            bool _respuesta;
            bool _config;
            string _terminal;
            //190.192.169.210
            string _char = "#"; //IP Publica,       LAN                 wifi
            string _ipclover = "190.192.169.210";//"192.168.0.25"; // "192.168.0.250";
            Int32 _port = 0;
            short _cuotas = 0;
            Random random = new Random();
            string _horas = "";
            string _admin;
            string _operador = "ADMIN";
            double _monto = 50.123;
            bool _print = false;
            string _comercio;
            string imp = _monto.ToString("F");
            //var str = _monto.ToString();
            //var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
            //ci.NumberFormat.NumberDecimalSeparator = ",";
            ////ci.NumberFormat.NumberGroupSeparator = ".";
            //ci.NumberFormat.NumberGroupSizes = new[] { 0 };
            //var result = decimal.Parse(str).ToString("N", ci);
            ////
            //var s = p.GetValueFromDataFile("KEY");

            string _fechaaux = DateTime.Now.Date.ToString(new CultureInfo("fr-FR"));
            string _stringkeyvalue = "FCHMOV="+ _fechaaux;
            string _randow = DateTime.Now.ToLongTimeString().Replace(":", "").Substring(0, 4);
            string _factura = DateTime.Now.ToString("ddMMyyHHmmss");
            p.StringKeyPairValue(_stringkeyvalue, "#");
            var a = p.GetStringKeyValue;
            
            //Utilities.DateString("25/10/2021", TipoFuncion.Credito, out _date);
            //_respuesta = Utilities.DateValidate(_date, TypeDate.ja_JP);
            /******prueba******/
            Console.WriteLine("Ingresar tipo de Función: General:0,Débito:2, Credito:3, Gerencial:110, ReimprimirUltimo:114, Pendientes: 130, PendienteID:131");
            tpfuncion = (TipoFuncion)int.Parse(Console.ReadLine());
            Console.WriteLine("Modo Developer-Manual: S,N?");
            p.ConsoleMode = (Console.ReadLine().ToUpper() == "S")?true :false;
            Console.WriteLine("Cantidad de cuotas");
            _cuotas = short.Parse(Console.ReadLine());
            if (tpfuncion == TipoFuncion.ReimpresionUltimo)
            {
                tf = TipoFuncion.ReimpresionUltimo;
                tfpago = TipoFormaPago.ReimprimirUltimo;
                tptj = TipoTarjeta.TCreditoDIG;
                //p.HHMMSS = "124700";
                //_fechaaux = "09/02/2022";
                _monto = 0.01;
                _factura = "0";

            }
            else
            {
                tf = tpfuncion;
                if (tf == TipoFuncion.Credito)
                {
                    if (_cuotas > 1)
                        tfpago = TipoFormaPago.TcreditoTodas;
                    else
                        tfpago = TipoFormaPago.TCreditoEFT;
                    tptj = TipoTarjeta.TCreditoMAG;
                    tfdev = TipoFuncion.DevolucionTCMAG;
                }
                else if (tf == TipoFuncion.Debito)
                {
                    if (_cuotas > 1)
                        tfpago = TipoFormaPago.TDebitoTodas;
                    else
                        tfpago = TipoFormaPago.TDebitoEFT;
                    tptj = TipoTarjeta.TDebitoMAG;
                    tfdev = TipoFuncion.DevolucionTDMAG;

                }
                else
                {
                    tfpago = TipoFormaPago.TcreditoTodas;
                    tptj = TipoTarjeta.TCreditoMAG;
                }
               
            }

            if (_tp == TipoPinPad.Clover)
            {
                _terminal = "SE000003";
                _port = 9000;
                tfdev = TipoFuncion.DevolucionGral;
                _admin = "1234";
                _config = true;
                _comercio = "00000004";
                //_factura = "FB-0002";
            }
            else
            {
                _terminal = "SE000002";
                _port = 9;
                //tfdev = TipoFuncion.DevolucionTCMAG;
                emptj = EmpTarjeta.Visa;
                _admin = "ADMIN";
                _config = false;
                _comercio = "00000004";
                p.PlandDeCuotas = 1;
                //tf = TipoFuncion.CargaTablaPinPad;

            }
            
            string _comporig = "";
            //PinPad p = new PinPad(_nrofactura: _factura, _nrocuit: "30646941136", _ipserver: "52.67.141.229", _comercioid: "00000004", _terminalid: "SE000001", _monto: 1.00, "");

            //cofig Clover           "190.192.169.184"
            p.TimeOutSec = 360;
            p.ConfigSiteSitef("52.67.141.229", _terminal, _comercio, "30646941136");// ERROR_DE_COMANDO
            p.Imprimir = _print;
            p.TiempoEsperaCopia = 0;
            p.DebugPrint = true;
            p.CodSupervisor = _admin;
            p.CodOperador = _operador;

            if (_config)
            {
                p.ConfigTransaction(TipoFuncion.ConfigClover, TipoOperacion.General, _tp, _port, _ipclover);
                _respuesta = p.Begin(_factura, _monto, TipoFormaPago.TcreditoTodas, TipoTarjeta.TCreditoDIG, emptj, _fechaaux, NroDeCuotas: _cuotas, _admin);
            }
            
            //Factura
            p.TimeOutSec = 360;
            //52.67.141.229:10289
            p.ConfigSiteSitef("52.67.141.229", _terminal, _comercio, "30646941136");
            p.Imprimir = _print;
            p.TiempoEsperaCopia = 0;
            p.ConsoleWrite = true;
            p.CodSupervisor = _admin;
            p.CodOperador = _operador;
            //p.ConfigTransaction( TipoFuncion.Gerencial, TipoOperacion.General, _tp, _port, _ipclover);
            p.ConfigTransaction(tf, TipoOperacion.General, _tp, _port, _ipclover);
            if (p.TipoDePinPad == TipoPinPad.PinPad)
                p.WriteIniFile("SiTef", "PortaSiTef", "10289");
            p.StringKeyPairValue(_stringkeyvalue, _char);
            _respuesta = p.Begin(_factura, _monto, tfpago, tptj, emptj, _fechaaux, NroDeCuotas: _cuotas, _operador );
            if (!_respuesta)
            {
                Console.WriteLine("Error: {0},{1}", p.ErrorLogCode, p.ErrorLogDescrp);
            }
            else
            {
                Console.WriteLine("Cupo: {0}, Código de Aut.: {1}, Código Autorización Tarjeta: {2}, Código de TR(para devoluciones): {3}", p.NroCupon, p.CodAutorizacion, p.CodAtorizaTarjeta, p.CodTransSitef);
                Console.WriteLine("Código de tarjeta: {0}, primeros 6: {1}, ultimpos 4: {2}", p.CodTarjeta, p.PrimerosDigitos, p.UltimosDigitos);
            }
            Console.Read();

            //read file .dat
            
      
            p.GetKeysValuesFromDataFile();
            //Ultimo pendiente
            p.GetLastTrPending("#",";");
            string fec = p.DateLastTrPending();
            string comp = p.CompLastTrPending();
            string importe = p.ImportLastTrPending();

            p.Dispose();

            /*Finalizar pendiente*/
            //string pending = p.GetListPending("#",";");
            //tf = TipoFuncion.PendienteID;
            //p.CancelListPending("Date=20220125;Comprobante=250020","#",";");
            //IniciarTR(p);

            /*******************************/

            _comporig = p.CodTransSitef;
            _factura = "NCA-" + _randow.Substring(_randow.Length - 4);
            _stringkeyvalue = p.GetStringKeyValue;
            //Anulación - Devolución
            p.Imprimir = _print;
            p.TiempoEsperaCopia = 0;
            p.ConsoleWrite = true;
            p.CodSupervisor = _admin;
            p.CodOperador = _operador;
            //Clover la función de devolucióin es la gral=200
            p.ConfigTransaction(tfdev, TipoOperacion.Devolucion, _tp, _port, _ipclover);
            p.StringKeyPairValue(_stringkeyvalue, _char);
            _respuesta = p.Begin(_factura, _monto , tfpago, tptj, emptj, "", NroDeCuotas: _cuotas, _operador , _comporig);

            if (!_respuesta)
            {
                Console.WriteLine("Error: {0},{1}", p.ErrorLogCode, p.ErrorLogDescrp);
            }
            else
            {
                Console.WriteLine("Cupo: {0}, Código de Aut.: {1}, Código Autorización Tarjeta: {2}, Código de TR(para devoluciones): {3}", p.NroCupon, p.CodAutorizacion, p.CodAtorizaTarjeta, p.CodTransSitef);
                Console.WriteLine("Código de tarjeta: {0}, primeros 6: {1}, ultimpos 4: {2}", p.CodTarjeta, p.PrimerosDigitos, p.UltimosDigitos);
                _horas = p.HHMMSS;
                _fechaaux = p.Fecha;
            }
            Console.Read();
         
            p.Dispose();
            /*

            if (!_respuesta)
            {
                return;
            }
            else
                //Devolucion
                p.Imprimir = _print;
            p.CodSupervisor = "01";
            var _comprobateori = p.CodTransSitef;
            p.TiempoEsperaCopia = 2000;
            p.ConfigTransaction(TipoFuncion.DevolucionTCMAG, TipoOperacion.Devolucion, TipoPinPad.PinPad, PuertoCOM: 9);
            _respuesta = p.Begin(_comprobateori, 1.0, TipoFormaPago.TcreditoTodas, TipoTarjeta.TCreditoMAG, EmpTarjeta.Visa, _fechaaux, NroDeCuotas: _cuotas, "AFLORE", _comprobateori);

            if (!_respuesta)
            {
                Console.WriteLine("Error: {0},{1}", p.ErrorLogCode, p.ErrorLogDescrp);
            }
            else
            {
                Console.WriteLine("Cupo: {0}, Código de Aut.: {1}, Código Autorización Tarjeta: {2}, Código de TR(para devoluciones): {3}", p.NroCupon, p.CodAutorizacion, p.CodAtorizaTarjeta, p.CodTransSitef);
                Console.WriteLine("Código de Tarjeta.: {0}", p.CodTarjeta);
                _horas = p.HHMMSS;
                _fechaaux = p.Fecha;
            }
            Console.Read();

        */
            //bool IniciarTR(PinPad _p)
            //{
            //    p.TimeOutSec = 360;
            //    //52.67.141.229:10289
            //    p.ConfigSiteSitef("52.67.141.229", _terminal, _comercio, "30646941136");
            //    p.Imprimir = _print;
            //    p.TiempoEsperaCopia = 0;
            //    p.ConsoleWrite = true;
            //    p.CodSupervisor = _admin;
            //    p.CodOperador = _operador;
            //    //p.ConfigTransaction( TipoFuncion.Gerencial, TipoOperacion.General, _tp, _port, _ipclover);
            //    p.ConfigTransaction(tf, TipoOperacion.General, _tp, _port, _ipclover);
            //    if (p.TipoDePinPad == TipoPinPad.PinPad)
            //        p.WriteIniFile("SiTef", "PortaSiTef", "10289");
            //    p.StringKeyPairValue(_stringkeyvalue, _char);
            //    return p.Begin(_factura, _monto, tfpago, tptj, emptj, _fechaaux, NroDeCuotas: _cuotas, _operador);

            //}
        }

        
    }
}
