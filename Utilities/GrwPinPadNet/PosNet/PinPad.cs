using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SPIClient;
using System.Net;
using System.Reflection;
using System.Globalization;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace GrwPinPadNet
{
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("731ae2ab-ed2e-40d7-932d-baca312b2b6e"), ComVisible(true)]
    public class PinPad : IPosNet, IDisposable
    {
        #region Import
        [DllImport("CliSiTef32I.dll")]//, EntryPoint = "FinalizaFuncaoSiTefInterativoEx", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int FinalizaFuncaoSiTefInterativoEx(short ConfirmationFlag, string SaleInvoice, string InvoiceDate, string InvoiceTime,
        string AdditionalParam);

        [DllImport("CliSiTef32I.dll")] //, EntryPoint = "IniciaFuncaoSiTefInterativo", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int IniciaFuncaoSiTefInterativo(int Function, string Value, string SaleInvoice, string InvoiceDate, string InvoiceTime, string Operator,
        string AdditionalParam);
        //
        [DllImport("CliSiTef32I.dll")]
        static extern int ConfiguraIntSiTefInterativoEx(string SiTefIP, string MerchantID, string TerminalID, short Reservado, string AdditionalParam);
        //
        [DllImport("CliSiTef32I.dll")]
        static extern int ContinuaFuncaoSiTefInterativo(out int Comando, out int TipoCampo, out short TamMinimo, out short TamMaximo, byte[] Buffer, int TamBuffer, int Continua);

        [DllImport("CliSiTef32I.dll")]
        static extern int VerificaPresencaPinPad();

        [DllImport("CliSiTef32I.dll")]
        static extern int ImprimePOS(string lpcDados, string lpcParamAdic);

        [DllImport("CliSiTef32I.dll")]
        static extern int ObtemQuantidadeTransacoesPendentes(string DataFiscal, string CupomFiscal);

        [DllImport("kernel32.dll", EntryPoint = "SetCommTimeouts", SetLastError = true)]
        static extern bool SetCommTimeouts(SafeHandle hFile, int timeouts);

        [DllImport("CliSiTef32I.dll")]
        static extern int LeSimNaoPinPad(string mensagem);
        #endregion

        #region Propertys
        string _errorlogcode;
        string _errorlogdescrp;
        double _import;
        TipoFuncion _funcion = TipoFuncion.PGeneral;
        string _operador;
        string _factura;
        string _cupon;
        EmpTarjeta _codtarjeta;
        string _codautorizacion;
        short _cuotas;
        short _plancuotas;
        string _idtransaccion;
        string _terminal_id;
        string _comercio_id;
        string _cuit;
        string _cuitisv;
        string _ipclover;
        string _codautotarjeta;
        string _codsupervisor;
        string _nsusitef;
        string _orderid;
        string _paymentID;
        string _primerosdigitos;
        string _ultimosdigitos;
        int _puerto;
        int _transpending;
        double _timeoutsec;
        static int _cantidpendientes;
        static int _tiempoesperacopia;
        string _fecha;
        string _ipsitef;
        int _flag = -1;
        bool _true = false;
        bool _inicializado;
        bool _cancelpending;
        bool _finishauto = true;
        bool _yesnot = true;
        int retorno;
        string _date;
        string _dateini;
        string _hour = DateTime.Now.ToString("HHmmss");
        string _cuponorig;
        string _cuponcopia;
        string _codemparejamiento;
        string _args;
        string _listpending;
        string _listDelimiter = ";";
        string _fieldsDelimiter = "#";
        string _nexoDelimiter = "=";
        string _listcancelpending;
        Task<int> _task = null;
        List<Task<int>> _tasks;
        List<TransPending> _listtrans;
        protected struct StructKeyValue
        {
            public string cadena { get; set; }
            public string splitchar { get; set; }
        }
        List<Task<int>> ListTasks()
        {
            if (_tasks == null)
                _tasks = new List<Task<int>>();
            return _tasks;
        }
        StructKeyValue _stkeyvalue;
        readonly string _cuitdeveloper = "30646941136";
        static string _inifile = "Clisitef.ini";
        static string _datfile = "tmp_trans.dat";
        Dictionary<string, string> _dicdatafile;
        TipoOperacion _tipooperacion;
        TipoFormaPago _tipodepago;
        TipoTarjeta _tipotarjeta;
        TipoPinPad _tipopinpad;
        EmpTarjeta _codemptarjeta;
        TransPending _tr;
        //90 segundos default
        TimeSpan _ts;
        List<int> listretornos = new List<int> { 0, 255 };
        public double Importe { get => _import; }
        public string CodOperador { get => _operador; set => _operador = value; }
        public TipoFuncion Funcion { get => _funcion; }
        public string Factura { get => _factura; }
        public string NroCupon { get => _cupon; }
        public EmpTarjeta CodTarjeta { get => _codtarjeta; }
        public string CodAutorizacion { get => _codautorizacion; }
        public short Cuotas { get => _cuotas; set => _cuotas = value; }
        public short PlandDeCuotas { set => _plancuotas = value; }
        public string IDTransaccion { get => _idtransaccion; }
        public TipoFormaPago FormaDePago { get => _tipodepago; }
        public TipoTarjeta TipoDeTarjeta { get => _tipotarjeta; }
        public EmpTarjeta CodempTarjeta { set => _codemptarjeta = value; }
        public string CUIT { get => _cuit; }
        public string CUITISV { set => _cuitisv = value; }
        public string IPSITEF { get => _ipsitef; }
        public string COMERCIO_ID { get => _comercio_id; }
        public string TERMINAL_ID { get => _terminal_id; }
        public string ErrorLogCode => _errorlogcode;
        public string ErrorLogDescrp => _errorlogdescrp;
        public string IPClover { get => _ipclover; }
        public TipoPinPad TipoDePinPad { get => _tipopinpad; }
        public double TimeOutSec { set => _timeoutsec = value; }
        public string CodAtorizaTarjeta { get => _codautotarjeta; }
        public string PrimerosDigitos { get => _primerosdigitos; }
        public string UltimosDigitos { get => _ultimosdigitos; }
        static protected string CuponAux { get; set; }
        static protected string FechaAux { get; set; }
        public string CodSupervisor { set => _codsupervisor = value; }
        public TipoOperacion TipoOperacion { get => _tipooperacion; }
        public string CodTransSitef { get => _nsusitef; set => _nsusitef = value; }
        public bool Imprimir { set; get; }
        public bool FinishAuto{set => _finishauto = value; }
        public string Fecha { set => _fecha = value; get => _fecha; }
        public string HHMMSS { get => _hour; set => _hour = value; }
        public int TiempoEsperaCopia { get => _tiempoesperacopia; set => _tiempoesperacopia = value; }
        public bool ConsoleWrite { get; set; }
        public int CantidadPendientes { get => _cantidpendientes; }
        public string SetIniFile { set => _inifile = value; }
        public bool DebugPrint { set =>ConsoleWrite = value ; }
        public bool ConsoleMode { get; set; }
        public string CodEmparejamiento { get => _codemparejamiento; }
        public string OrderID { set => _orderid = value; get => _orderid; }
        public string PaymentID { set => _paymentID = value; get => _paymentID; }
        public string Lote { set; get; }            
        public string GetListPending (string ListDelimiter, string FieldsDelimiter)
        {
            bool _process = true;
            if (_listpending != null)
                if (_listpending.Length > 0)
                    _process = false;
            if(_process)
            {
                Func<TransPending, string> splistr = str => string.Concat(str.Date, FieldsDelimiter, str.Comprobante, FieldsDelimiter, str.Import);
                _listpending = Utilities.ConcatStrList(_listtrans, ListDelimiter, FieldsDelimiter);
                _listtrans = null;
            }
                   

            return _listpending;
        }
        public string GetLastTrPending(string ListDelimiter, string FieldsDelimiter)
        {
            bool _process = true;
            string _lastdoc = "";
            if (_listpending != null)
                if (_listpending.Length > 0)
                    _process = false;
            if (_process)
            {
                _tr = Utilities.GetLastPending(_listtrans);
                _lastdoc = _tr.Comprobante;


            }
            return _lastdoc;
        }
        public void CancelListPending(string ListPending, string ListDelimiter, string FieldsDelimiter)
        {
            (_listcancelpending, _listDelimiter, _fieldsDelimiter) = (ListPending, ListDelimiter, FieldsDelimiter);
            _cancelpending = true;
        }
        /******************************************/
        /* Datos del último comprobante pendiente */
        /******************************************/
        public string DateLastTrPending()
        {
            string fecha = "";
            if (_tr != null)
                fecha = _tr.Date;
            return fecha;
        }
        public string HourLastTrPending()
        {
            string text = "";
            if (_tr != null)
                text = _tr.Hour;
            return text;
        }
        public string CompLastTrPending()
        {
            string text = "";
            if (_tr != null)
                text = _tr.Comprobante;
            return text;
        }
        public string ImportLastTrPending()
        {
            string text = "";
            if (_tr != null)
                text = _tr.Import;
            return text;
        }
        /// <summary>
        /// Recupera string con el formato: key1=value1;keyx=valuex
        /// </summary>
        public string GetKeysValuesFromDataFile(string ListDelimiter = "" , string nexoDelimiter = "")
        {   
            return Utilities.GetKeysValuesFromDataFile(_datfile, ListDelimiter == ""? _listDelimiter : ListDelimiter, nexoDelimiter == ""? _nexoDelimiter : nexoDelimiter);
        }
        public string GetValueFromDataFile(string Key, string ListDelimiter = "", string NexoDelimiter = "")
        {
            return Utilities.GetValueFromDataFile(ListDelimiter == ""? _listDelimiter : ListDelimiter, NexoDelimiter == ""? _nexoDelimiter : NexoDelimiter, Key);
        }
        /******************************************/
        public bool Inicializado { get => _inicializado; set => _inicializado = value; }
        public string GetStringKeyValue { get => (string)_stkeyvalue.cadena; }
        public void StringKeyPairValue(string Cadena, string SplitChar)
        {
            _stkeyvalue = new StructKeyValue() { cadena = Cadena, splitchar = SplitChar };
        }
       
        public IReturnValue ReturnValue
        {
            set; get;
        }
        //Constructor
        public PinPad()
        {
            Inicializar(TipoInicializacion.Construct);
            //Asignarl el path de la palicación al archivo .dat
            _datfile = Utilities.GetPathFile(_datfile);
        }
        public void WriteIniFile(string Section, string Key, string Value)
        {
            Utilities.IniWriteValue(_inifile, Section, Key, Value);
            //prueba
            /*
            for (int i = 0; i <= 5; i++)
            {
                if (i == 3)
                    if (ReturnValue != null)
                        ReturnValue.ReturnStringValue("for i: " + i.ToString());
            }
            */
        }
        public string AssemblyDirectory{get=> Utilities.AssemblyDirectory; }
        public void ConfigSiteSitef(string IPSitef, string TerminalID, string Comercio, string Cuit, string CUITisv = "", string args = "")
        {
            Inicializar(TipoInicializacion.ConfigurationSitef);
            (_ipsitef, _terminal_id, _comercio_id, _cuit, _cuitisv, _args) = (IPSitef, TerminalID, Comercio, Cuit, CUITisv, args);
        }
        public void ConfigTransaction(TipoFuncion TipoDeFuncion, TipoOperacion TipoDeOperacion = TipoOperacion.General, TipoPinPad Dispositivo = TipoPinPad.PinPad, int PuertoCOM = 0, string IPClover = "")
        {
            Inicializar(TipoInicializacion.ConfigurationIni);
            (_funcion, _tipooperacion, _tipopinpad, _puerto, _ipclover) = (TipoDeFuncion, TipoDeOperacion, Dispositivo, PuertoCOM, IPClover);
            ConfigClisitefIni();
            //Crea o inicializa el archivo .dat, para que no exista datos anteriores
            if (_funcion != TipoFuncion.PendientesTodos && _funcion != TipoFuncion.PendienteID)
                Utilities.WriteDataFile(_datfile, "", "", "");
        }
        public bool Begin(string NroComprobante, double Monto, TipoFormaPago TipoPago, TipoTarjeta TipoTarjeta, EmpTarjeta CodTarjeta, string FechaComprobante = "", short NroDeCuotas = 0, string Usuario = "",string CompNSU = "")
        {
            Inicializar(TipoInicializacion.Transaction);
            (_factura, _import, _tipodepago, _tipotarjeta, _codemptarjeta,  _fecha, _cuotas, _operador, _nsusitef) = (NroComprobante, Monto, TipoPago, TipoTarjeta, CodTarjeta, FechaComprobante, NroDeCuotas, Usuario, CompNSU);
            _true = false;
             retorno = _flag;
            if (_tipooperacion == TipoOperacion.Devolucion)
                GetKeyValue(_stkeyvalue);
            //Para cualquier operación en la rutina "ConfiguraIntSiTefInterativoEx" debe ser YYYYMMDD
            Utilities.DateString("", TipoFuncion.PGeneral, TipoOperacion.General, TipoPinPad.Ninguno, out _dateini);
            //Convertir según la función a implementar. Devoluciones: DDMMYYYY
            if (!Utilities.DateString(_fecha, _funcion, _tipooperacion, _tipopinpad, out _date))
            {
                BuildMessage("GR000068", "Fecha de devolución inválida");
                return false;
            }
            _hour = DateTime.Now.ToString("HHmmss");
            //Termino las tareas anteriores
            FinishListTasks();
            if (!_inicializado)
            {
                if (Validaciones(TipoInicializacion.ConfigurationSitef))
                {
                    //Configuración del sitio Sitef
                    _task = Task<int>.Run(() => CofigurarSitef(_args));
                    retorno = TaskWaitOrGetException<int>(_task, _ts, "ConfiguraIntSiTefInterativoEx");
                }
            }
            else
                retorno = 0;
            
            if (retorno != 0)
            {
                if(_errorlogcode.Trim().Length == 0) BuildMessage("GR000068", "EL sitio no se encuentra inicializado");
                return false;
            }
           
            if (Validaciones(TipoInicializacion.Transaction))
            {
                if (IsSale(_funcion))
                    SetReturnValueString(0, "IniciaFuncaoSiTefInterativo-CuponFiscal: " + _factura.ToString());
                _task = Task<int>.Run(() => IniciarFuncion(_funcion, _import, _factura, _dateini, _hour, _operador));
                retorno = TaskWaitOrGetException<int>(_task, _ts, "IniciaFuncaoSiTefInterativo");
                if (retorno == 10000)
                {
                    //Loop mientras la función ContinuaFuncaoSiTefInterativo devuelve 10000
                    //_task = Task.Run(() => ContinuarFuncion(out string[] s));
                    //retorno = TaskWaitOrGetException<int>(_task, _ts, "ContinuaFuncaoSiTefInterativo");
                    //Si se ejecuta async los mensaje se muestran con Delay
                    retorno = ContinuarFuncion(out string[] s, out _listtrans);
                }
                bool boollist = false;
                if (_funcion == TipoFuncion.PendienteID && retorno == -13)
                    boollist = true;
                else
                    if(listretornos.IndexOf(retorno) >=0)
                        boollist = true;

                if (boollist)
                {
                    if (_tipopinpad != TipoPinPad.Clover)
                    {
                        if (IsPrint(_cuponcopia,_cuponorig , Imprimir, _tipodepago))
                        {
                            _task = ImprimirCupon(_cupon, _cuponorig, _cuponcopia, _tiempoesperacopia);
                            _tasks = ListTasks();
                            _tasks.Add(_task);
                        }
                    }
                }
                if (!_finishauto)
                {
                    SetReturnValueString(9999, "Finalizar?");
                    Thread.Sleep(3000);
                }
                if ((_finishauto && FinalizaFuncion(_funcion)) || (!_finishauto && _yesnot))
                {
                    if (_tipopinpad != TipoPinPad.Clover)
                    {
                        if (_cancelpending)
                            retorno = FinalizarComprobantes(false, _listcancelpending, _listDelimiter, _fieldsDelimiter);
                        else
                        {
                            _task = Task<int>.Run(() => FinalizaFuncaoSiTefInterativoEx((short)(boollist ? 1 : 0), _factura, (string)_dateini, (string)_hour, ""));
                            retorno = TaskWaitOrGetException<int>(_task, _ts, "FinalizaFuncaoSiTefInterativoEx");
                        }
                    }
                    _true = (retorno == 0 && boollist);
                    if (_true)
                    {
                        string _char = _stkeyvalue.splitchar;
                        //_stkeyvalue.cadena = "FCHMOV=" + _fecha;
                        if (_tipopinpad == TipoPinPad.Clover)
                            _stkeyvalue.cadena = "ORDERID=" + _orderid + _char + "PAYMENTID=" + _paymentID;
                    }
                    else
                    {
                        _cantidpendientes = 0;
                        if (_tipopinpad != TipoPinPad.Clover)
                        {
                            _cantidpendientes = ObtemQuantidadeTransacoesPendentes(_date, _factura);
                            //Consultar si la transacción quedó pendiente.
                            //Devuelve la cantidad de registros pendientes para la fecha y el comprobante consultado
                            //_cantidpendientes = ObtemQuantidadeTransacoesPendentes(_date, _factura);
                            if (_cantidpendientes > 0)
                            {
                                _true = false;
                                //GR000459      La operación envíada no fue confirmada por el sitio, su estado quedó pendiente de confirmación.
                                BuildMessage("GR000459");
                            }
                        }
                    }
                }
                else
                {
                    _true = (retorno == 0 && boollist);
                }
                _inicializado = _true;

                Inicializar(TipoInicializacion.Finish);
            }

            //toque
            if ((_funcion == TipoFuncion.PendientesTodos || _funcion == TipoFuncion.PendienteID) &&  ConsoleWrite)
               _listtrans.ForEach (item => Console.WriteLine("{0},{1},{2}",item.Comprobante, item.Date, item.Import));
  

            return _true;
        }
         public void Dispose()
         {
            Dispose(true);
            GC.SuppressFinalize(this);
         }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                FinishListTasks();
            }
            // free native resources if there are any.
        }

        #endregion
        #region Métodos
        /// <summary>
        /// Comienza el proceso de transacción entre el Posnet -> PDV -> el sitio Sitef
        /// </summary>
        /// <returns>0: si fue satisfactorio, -1: no satisfactorio</returns>

        #endregion
        #region Funciones Privadas

        int TaskWaitOrGetException<T>(Task<T> task, TimeSpan ts, string _method = "")
        {
            int _r = _flag;
            try
            {
                if (!(task.Wait(ts) && task.IsCompleted))
                {
                    _r = BuildMessage("GR000068", "La tarea " + (_method.Length >0 ? _method : string.Empty) + " superó el tiempo de timeout " + ts.ToString() + "\"", _flag);
                }
                else
                    _r = (int)Convert.ChangeType(task.Result, typeof(int));
            }
            catch (AggregateException ae)
            {
                if (_errorlogcode.Trim().Length == 0)
                {
                    ae.Handle((x) =>
                    {
                        if (x is UnauthorizedAccessException) // This we know how to handle.
                            BuildMessage("GR000068", "permission to access all folders in this path");
                        else
                            BuildMessage("GR000068", x.Message);
                        return true;
                    });
                }
                _r = _flag;
            }
            return _r;
        }

        //Inicialición del sitio Sitef (una vez por día)
        int CofigurarSitef(string args)
        {
            StringBuilder param = new StringBuilder("[");
            param.Append("CUIT=" + string.Format("{0}", _cuit));
            param.Append(";CUITISV=" + string.Format("{0}", _cuitisv));
            if (args.Length>0)
            {
                    //Por ejemplo: param.Append(";PortaPinPad=9");
                    param.Append(";" + args);
            }
            
            param.Append(";]");
            
            return ConfiguraIntSiTefInterativoEx(
                string.Format("{0}", _ipsitef),
                string.Format("{0}",_comercio_id),
                string.Format("{0}", _terminal_id),
                0, param.ToString());
        
        }
        
        int IniciarFuncion(TipoFuncion TipoDeFuncion, double Monto, string NroDocumento, string dataStr, string horaStr, string Usuario = "")
        {
            int rpta = 0;
            TipoFormaPago _tipopago;
            StringBuilder _parametrosAdicionais = new StringBuilder();
            string valorStr= Utilities.ConvertNumber<double>(Monto, ","); 
            if (Enum.TryParse(_tipodepago.ToString(), out _tipopago))// && _tipopinpad != TipoPinPad.Clover)
            {
                _parametrosAdicionais.Append(RestriccionesMediosDePago(_tipotarjeta, _tipopago,  _cuotas));
            }
            
            rpta = IniciaFuncaoSiTefInterativo((int)TipoDeFuncion, valorStr, NroDocumento, dataStr, horaStr, _operador, _parametrosAdicionais.ToString());
            return rpta;
        }

        int ContinuarFuncion(out string[] datosimprimir, out List<TransPending> listPending)
        {
            byte[] valorBuffer = new byte[20000];
            int result;
            int continua = 0;
            int _continua = continua;
            string captionMenu = "";
            string captionCarteiraDigital = "";
            string[] aux = new string[0];
            short _veces = 0;
            int _ultimotipocampo = 0;
            string _ultimomensaje = "";
            string respostaSitef = "";
            short _reintento = 0;
            int _pending = 1;
            datosimprimir = new string[0];
            List<char> listchar = new List<char> { '1', 's', 'S', 'y', 'Y' };
            /*******Lista y Clase para acumular las transacciones pendientes******/
            listPending = new List<TransPending>();
            TransPending clstrans = new TransPending();
            _dicdatafile = new Dictionary<string, string>();
            _dicdatafile.Add("FISVID", _factura);
            /*********************************************************************/
            //Comienza Loop
            do
            {
                result = ContinuaFuncaoSiTefInterativo(out int proximoComando, out int tipoCampo, out short tamanhoMinimo, out short tamanhoMaximo, valorBuffer, valorBuffer.Length, continua);
                string mensagem = Encoding.UTF8.GetString(valorBuffer).Replace("\0", "").Trim();
                
                respostaSitef = "";
                /*
                    (continua) de acuerdo con los siguientes códigos:
                    0 Continuar la transacción
                    1  Volver a recolectar el campo anterior, si es posible.
                    2  Cancelar el pago actual y guardar en la emoria los anteriores, si existen. Permite que
                    tales pagos se envíen a SiTef e incluso permite la adición de nuevos pagos. La devolución es válida
                    solo para el cobro del monto del pago y la fecha de vencimiento.
                    10000  Continuar la transacción (ver nota a continuación)
                    -1  Termina la transacción
                */

                if (continua == 1)
                    System.Threading.Thread.Sleep(4000);
                continua =  0;
                if (result == 10000)
                {
                    switch (proximoComando)
                    {
                        #region Case 0
                        case 0: //Está devolviendo um valor para, se desejado, ser armazenado pela automação
                            #region Trata Tipo de Campo
                            if (tipoCampo == 0)
                            {
                                //if (ConsoleWrite)
                                //{
                                //    Console.WriteLine("Comienza la transacción con Sitef - Mensaje: {0}", mensagem);
                                //}
                            }
                            else if (tipoCampo == 1)
                            {
                                //if (ConsoleWrite)
                                //{
                                //    Console.WriteLine("Transacción confirmada. Buffer: {0}", mensagem);
                                //}
                                _dicdatafile.Add("TR", mensagem);
                                _dicdatafile.Add("CUPON", _cupon);
                                _dicdatafile.Add("DATE", _date);
                                Utilities.WriteDataFile(_datfile, _listDelimiter, _nexoDelimiter, _dicdatafile);
                                CuponAux = _cupon;
                                FechaAux = _date;
                            }
                            //Medio de Forma y Pago que se seleccionó
                            else if (tipoCampo == 29 || tipoCampo == 30 || tipoCampo == 42 || tipoCampo == 43)
                            {
                                if (ConsoleWrite)
                                {
                                    Console.WriteLine("Tipo de medio de pago: {0}", tipoCampo);
                                }

                            }
                            //Reimpresiones 56 - 58: 56:general, 57:ultimo, 58 específico
                            else if (tipoCampo == 56)
                            {
                                // 0: confirmación
                                respostaSitef = "0";
                            }
                            else if (tipoCampo == 100)
                            {
                                string msgAut = mensagem.PadRight(4, '0');
                                string _grupo = msgAut.Substring(0, 2);
                                string _subgrupo = msgAut.Substring(2, 2);

                                switch (_grupo)
                                {
                                    //Tarjeta de débito
                                    case "01":
                                        if (_tipotarjeta == TipoTarjeta.TCreditoMAG)
                                        {
                                            //GR000364 El POS respondió: La tarjeta deslizada por el usuario no coincide con la pedida (Se envió ~1)
                                            continua = BuildMessage("GR000364", _codemptarjeta.ToString(), _flag);
                                        }
                                        break;
                                }

                            }
                            else if (tipoCampo == 105)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                string msgData = mensagem.Substring(6, 2) + mensagem.Substring(4, 2) + mensagem.Substring(0, 4);
                                //if (ConsoleWrite) Console.WriteLine(msgData);


                                string msgHora = mensagem.Substring(8);
                                //if (ConsoleWrite) Console.WriteLine(msgHora);

                            }
                            else if (tipoCampo == 106)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                if (!string.IsNullOrWhiteSpace(mensagem))
                                {
                                    //if (ConsoleWrite) Console.WriteLine(mensagem);
                                }
                            }
                            else if (tipoCampo == 107)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                if (!string.IsNullOrWhiteSpace(mensagem))
                                {
                                    captionCarteiraDigital = mensagem;

                                }
                            }
                            else if (tipoCampo == 108) //Marca de la tarjeta
                            {
                                //if (ConsoleWrite) Console.WriteLine("Marca de la tarjeta: {0}", mensagem);
                            }
                            else if (tipoCampo == 111)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                            }
                            //Datos del cupón a imprimir
                            else if (tipoCampo == 121)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                _cuponorig = mensagem;
                                if (mensagem.Contains("******"))
                                {
                                    var start = mensagem.IndexOf("*", 0) + 6;
                                    _ultimosdigitos = mensagem.Substring(start, 4);
                                }
                                //cupon
                                if ((int)_tipodepago >= 56 && (int)_tipodepago <= 58)
                                    _cupon = Utilities.GetNumberFromStr(mensagem, "cupon", 12, "\n");

                            }

                            else if (tipoCampo == 122)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                _cuponcopia = mensagem;
                            }
                            else if (tipoCampo == 123)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                            }
                            else if (tipoCampo == 131)
                            {
                                if (ConsoleWrite) Console.WriteLine(mensagem);
                            }
                            //132 Código de tarjeta
                            else if (tipoCampo == 132)
                            {
                                /*
                                00000 Outro, não definido
                                00001 Visa
                                00002 Mastercard
                                00003 Diners
                                00004 American Express
                                */

                                if (int.TryParse(mensagem.Substring(mensagem.Length - 2), out var _dig))
                                {
                                    if ((int)_codemptarjeta != _dig)
                                        //GR000456      La empresa emisora de la tarjeta no se corresponde con la enviada por el sistema. ~1
                                        continua = BuildMessage("GR000456", _codemptarjeta.ToString(), _flag);
                                    else
                                        _codtarjeta = _codemptarjeta;
                                }

                            }

                            //Transacción de pago NSU SiTef. Necesaria para las Devoluciones
                            else if (tipoCampo == 133)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                //PinPad
                                _nsusitef = string.Format("{0}", mensagem);
                            }
                            //Transacción de pago NSU HOST
                            else if (tipoCampo == 134)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                //Prueba Clover
                                _nsusitef = mensagem;
                            }
                            else if (tipoCampo == 135)
                            {
                                //if (ConsoleWrite) Console.WriteLine("Código de autorización de tarjeta: {0}", mensagem);
                                if (_tipopinpad == TipoPinPad.Clover)
                                    _codautorizacion = mensagem;
                                else
                                    _codautotarjeta = mensagem;
                            }
                            //Primeros 6 dígitos de la tarjeta
                            else if (tipoCampo == 136)
                            {
                                _primerosdigitos = mensagem;
                            }
                            //Nombre de la institución - Empresa de la tarjeta
                            else if (tipoCampo == 156)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                            }
                            else if (tipoCampo == 158)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                            }
                            //Nro del Cupon Pendiente
                            else if (tipoCampo == 160)
                            {
                                if (ConsoleWrite) Console.WriteLine("Cupon pdte:{0}", mensagem);
                                clstrans.Comprobante = mensagem;
                            }
                            //Fecha del comprobante pendiente
                            else if (tipoCampo == 163)
                            {
                                if (ConsoleWrite) Console.WriteLine(mensagem);
                                clstrans.Date = mensagem;
                            }
                            //Hora del comprobante pendiente
                            else if (tipoCampo == 164)
                            {
                                if (ConsoleWrite) Console.WriteLine(mensagem);
                                clstrans.Hour = mensagem;
                            }
                            /*Por el momento queda comentado, ya que la librería devuelve
                             * como cuota mínima (2), y este control no permitiría 
                             * el pago de 1 cuota con trarjeta de crédito*/
                            /************************************************************/
                            /*
                                //Bandera que indica si la fiannciación es del comerciante
                                else if (tipoCampo == 170)
                                    short.TryParse(mensagem, out _financiacomerciante);
                                //Número mínimo/máximo de cuotas del comerciante
                                else if (tipoCampo == 171 || tipoCampo == 172)
                                {
                                    if (_financiacomerciante == 1)
                                    {
                                        if (tipoCampo == 171)
                                            short.TryParse(mensagem, out _cuotamin);
                                        //Número máximo de cuotas
                                        else if (tipoCampo == 172)
                                        {
                                            if (int.TryParse(_cuotas.ToString(), out _))
                                                if (int.TryParse(mensagem, out int _cuotasmax))
                                                    if (_cuotas > _cuotasmax || _cuotas < _cuotamin)
                                                    {
                                                        //GR000069      El campo ~1 Tiene un valor inválido
                                                        var s = "La cantidad de cuotas(" + _cuotas.ToString() + @") no es válida. Min/Max: " + _cuotamin.ToString() + @"/" + _cuotasmax.ToString();
                                                        continua = BuildMessage("GR000068", s,
                                                            (_tipotarjeta == TipoTarjeta.TCreditoMAG || _tipotarjeta == TipoTarjeta.TDebitoMAG) ? _flag : 0);
                                                    }
                                        }
                                    }
                                }

                                //Bandera que indica si la fiananciación es del emisor de TJ
                                else if (tipoCampo == 174)
                                    short.TryParse(mensagem, out _financiatarjeta);

                                //Número mínimo/máximo de cuotas del comerciante
                                else if (tipoCampo == 175 || tipoCampo == 176)
                                {
                                    if (_financiatarjeta == 1)
                                    {
                                        if (tipoCampo == 175)
                                            short.TryParse(mensagem, out _cuotamin);
                                        //Número máximo de cuotas
                                        else if (tipoCampo == 176)
                                        {
                                            if (int.TryParse(_cuotas.ToString(), out _))
                                                if (int.TryParse(mensagem, out int _cuotasmax))
                                                    if (_cuotas > _cuotasmax || _cuotas < _cuotamin)
                                                    {
                                                        //GR000069      El campo ~1 Tiene un valor inválido
                                                        var s = "La cantidad de cuotas(" + _cuotas.ToString() + @") no es válida. Min/Max: " + _cuotamin.ToString() + @"/" + _cuotasmax.ToString();
                                                        continua = BuildMessage("GR000068", s,
                                                            (_tipotarjeta == TipoTarjeta.TCreditoMAG || _tipotarjeta == TipoTarjeta.TDebitoMAG) ? _flag : 0);
                                                    }
                                        }
                                    }
                                }
                            */
                            /***************************************************************/
                            //Total de Pendientes (Función 130)
                            else if (tipoCampo == 210)
                            {
                                if (ConsoleWrite) Console.WriteLine(mensagem);
                                if ((int)_funcion >= 130 && (int)_funcion <= 131)
                                {
                                    int.TryParse(mensagem, out _transpending);
                                }
                            }
                            //Tipo de función de la TR pendiente
                            else if (tipoCampo == 211)
                            {
                                clstrans.Function = mensagem;
                            }
                            //Cantidad Cuotas (si se usa PinPad)
                            else if (tipoCampo == 505)
                            {
                                //Si se usa PinPad-> TipoTarjeta:30, 43
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = (_cuotas > 0) ? _cuotas.ToString() : "1";
                            }
                            //Fecha de transacción (DDMMAAA) para ser cancelado/reimpreso
                            else if (tipoCampo == 515)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                            }
                            else if (tipoCampo == 516)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                            }
                            //Otras cuotas
                            else if (tipoCampo == 525)
                            {
                                //tarjeta digitada
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = (_cuotas > 0) ? _cuotas.ToString() : "1";
                            }

                            //Tipo de producto - Devuelto por el autorizador para identificar el tipo de monedero digital
                            //01:Crédito, 02: Débito/Instantáneo
                            else if (tipoCampo == 545)
                            {
                                if (ConsoleWrite) Console.WriteLine(mensagem);
                            }
                            else if (tipoCampo == 590)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                            }
                            else if (tipoCampo == 591)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                if (!string.IsNullOrWhiteSpace(mensagem))
                                {
                                    decimal valorRecarga = Convert.ToDecimal(mensagem) / 100M;
                                    //if (ConsoleWrite) Console.WriteLine(valorRecarga.ToString("N2"));

                                }
                            }
                            //Tipo de pago habilitado. 02 TEF Débito, 03 TEF Crédito
                            else if (tipoCampo == 731)
                            {
                                Console.WriteLine(mensagem);
                                Console.ReadLine();

                            }
                            else if (tipoCampo == 800)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);

                            }
                            else if (tipoCampo == 950) //Para Modulo SAT_NFCe INSTALADO - CNPJ da fonte pagadora (autorizador do cartão)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                if (!string.IsNullOrWhiteSpace(mensagem))
                                {

                                }
                            }
                            else if (tipoCampo == 951) //Para Modulo SAT_NFCe INSTALADO - Bandeira NFCE
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                if (!string.IsNullOrWhiteSpace(mensagem))
                                {

                                }
                            }
                            else if (tipoCampo == 952) //Para Modulo SAT_NFCe INSTALADO - Número de autorização NFCE
                            {
                                //if (ConsoleWrite) Console.WriteLine("Numero de Autorización NFCE: {0}", mensagem);
                                _codautorizacion = mensagem;
                            }
                            else if (tipoCampo == 953) //Para Modulo SAT_NFCe INSTALADO - Código da credenciadora
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                if (!string.IsNullOrWhiteSpace(mensagem))
                                {

                                }
                            }
                            //Últimos 4 dígitos de la tarjeta
                            else if (tipoCampo == 1190)
                            {
                                if (_ultimosdigitos.Length == 0)
                                    _ultimosdigitos = mensagem;
                            }
                            //Monto de la transacción pendiente
                            else if (tipoCampo == 1319)
                            {
                                if (ConsoleWrite) Console.WriteLine(mensagem);
                                clstrans.Import = mensagem;
                                //ultimo campo devuelto del comprobante pendiente, por eso se agregala clase a la lita
                                TransPending _clsaux = (TransPending)clstrans.Clone();
                                listPending.Add(_clsaux);
                                _clsaux.Dispose();
                                _pending++;
                                clstrans.Clean();
                            }
                            else if (tipoCampo == 2010)
                            {
                                //if (ConsoleWrite) Console.WriteLine("Código de autorizador: {0}", mensagem);
                            }
                            else if (tipoCampo == 2021)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                            }
                            else if (tipoCampo == 2022)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);
                                string msgAut = mensagem.PadRight(4, '0');
                                //if (ConsoleWrite) Console.WriteLine(msgAut.Substring(2, 2) + msgAut.Substring(0, 2));

                            }
                            else if (tipoCampo == 2023)
                            {
                                //if (ConsoleWrite) Console.WriteLine(mensagem);

                            }
                            else if (tipoCampo == 2090)
                            {
                                if (ConsoleWrite) Console.WriteLine("Tipo de Tarjeta leída: 00 Magnético, 01 y 02 VISA, 03 Con contacto, 06 Contactless, 99 Digitado. {0}", mensagem);

                            }
                            else if (tipoCampo == 2091)
                            {

                                //Estado de la última lectura de la tarjeta: 0 - Éxito, 1 - Error de retroceso, 2 - Solicitud requerida no apoyada
                                short _out;
                                if (short.TryParse(mensagem, out _out))
                                {
                                    if (_out != 0)
                                    {
                                        if (_out == 1)
                                        {
                                            //GR000451 No fue posible leer la tarjeta
                                            _errorlogcode = "GR000451";
                                        }
                                        else if (_out == 2)
                                        {
                                            //GR000452      La tarjeta debe ser insertada o apoyada en el PinPad.
                                            _errorlogcode = "GR000452";
                                        }
                                        _reintento += 1;
                                        continua = _reintento < 3 ? 1 : BuildMessage(_errorlogcode, _while: _flag);
                                    }
                                }

                                //if (ConsoleWrite) Console.WriteLine("Estado de la última lectura de la tarjeta: 0 - Éxito, 1 - Error de retroceso, 2 - Solicitud requerida no apoyada {0}", mensagem);
                            }
                            else if (tipoCampo == 2093)
                            {
                                if (ConsoleWrite) Console.WriteLine(mensagem);
                            }
                            else if (tipoCampo == 2333)//ID Transacción
                            {
                                //if(ConsoleWrite) Console.WriteLine("ID de la Transacción: {0}", mensagem);
                                _idtransaccion = mensagem;
                            }
                            /*
                                Devuelto justo después de la transacción de consulta de los bins. El valor 1 indica
                                que el autorizador puede manejar de manera diferente la transacción de ---débito---
                                convencional para el pago de cuentas
                            */
                            else if (tipoCampo == 2362)
                            {
                                /*
                                //if(ConsoleWrite) Console.WriteLine("ID de la Transacción: {0}", mensagem);
                                if( _cuotas >1)
                                {
                                    //GR000069      El campo ~1 Tiene un valor inválido
                                    _errorlogcode = "GR000069";
                                    _errorlogdescrp = "Cantidad de cuotas";
                                    continua = -1;
                                }
                                */
                            }
                            /*
                                Devuelto justo después de la transacción de consulta de los bins. El valor 1 indica
                                que el autorizador puede manejar de manera diferente la transacción de ---crédito---
                                convencional para el pago de cuentas
                            */
                            else if (tipoCampo == 2364)
                            {

                            }
                            else if (tipoCampo == 2470)
                            {
                                if (ConsoleWrite) Console.WriteLine("Cantidad de decimales: ", mensagem);
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                            }
                            else if (tipoCampo == 2977) //Tipo de PinPad (5=Clover)
                            {
                                //if (ConsoleWrite) Console.WriteLine("Tipo de PinPad - 5 Clover {0}", mensagem);
                            }

                            else if (tipoCampo == 4077)//Cupón
                            {
                                _cupon = mensagem;
                                //if (ConsoleWrite) Console.WriteLine("Número de cupón: {0}", mensagem);
                            }
                            else if (tipoCampo == 4100)//Error de comunicación
                            {
                                //GR000453      No es posible la conexión con el servidor del proveedor
                                continua = BuildMessage("GR000453", _while: _flag);
                            }
                            /*Tipo de tarjeta PinPad. 
                            Datos de respuestas del Clover. En este campo se envían los datos recibidos en
                            repuesta a una transacción con la SPA. Si el dato no está presente en repuesta,
                            no es enviado. Cada dato es “clave=valor”. Los datos están separados por “|”.
                            */
                            else if (tipoCampo == 4147)
                            {
                                if (_tipopinpad == TipoPinPad.Clover)
                                {
                                    try
                                    {
                                        Dictionary<string, string> dic =
                                            mensagem.Split('|')
                                            .Select(x => x.Split('=')).Where(y => !string.IsNullOrWhiteSpace(y[0]))
                                            .GroupBy(x => x[0].ToUpper(), x => x[1])
                                            .ToDictionary(x => x.Key, x => x.First());
                                        //_nsusitef = dic["EXTERNALPAYMENTID"];
                                        _orderid = dic["ORDERID"];
                                        _paymentID = dic["PAYMENTID"];
                                        _cupon = dic["RECEIPTNUMBER"];
                                        Lote = dic["BATCHNUMBER"];
                                        captionMenu = "Clover-ExternalpaymentId: " + _nsusitef + "\n";
                                        captionMenu = captionMenu + "Clover-orderId: " + _orderid + "\n";
                                        captionMenu = captionMenu + "Clover-paymentId: " + _paymentID;
                                    }
                                    catch (Exception e)
                                    { Console.WriteLine(e.Message); }
                                }
                                else
                                {
                                    captionMenu = "Tipo de Tarjeta: " + mensagem;
                                }
                                //if (ConsoleWrite) Console.WriteLine(captionMenu);
                            }
                            else if (tipoCampo == 4160)//Se debe informa la IP del Clover o su nombre Host 
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = string.Format("{0}", _ipclover);
                            }
                            //Se espera lectura de tarjeta
                            else if (tipoCampo == 5000)
                                if (ConsoleWrite) Console.WriteLine(mensagem);
                                //Se espera digitado del usuario
                                else if (tipoCampo == 5001)
                                    if (ConsoleWrite) Console.WriteLine(mensagem);
                                    //Se espera confirmación
                                    else if (tipoCampo == 5002)
                                        if (ConsoleWrite) Console.WriteLine(mensagem);

                                        else if (tipoCampo == 5036)// Antes da leitura do cartão magnético
                                        { }
                                        else if (tipoCampo == 5037)// Antes da leitura do cartão com CHIP 
                                        { }
                                        else if (tipoCampo == 5038)// Antes da remoção do cartão com CHIP 
                                        { }
                                        else if (tipoCampo == 5039)// Antes da coleta da senha no pinpad 
                                        { }
                                        else if (tipoCampo == 5040)// Antes de abrir a comunicação com o PinPad 
                                        { }
                                        else if (tipoCampo == 5041)// Antes de fechar a comunicação com o PinPad 
                                        { }
                                        else if (tipoCampo == 5042)// Deve bloquear recursos para o PinPad 
                                        { }
                                        else if (tipoCampo == 5043)// Deve liberar recursos para o PinPad 
                                        { }
                                        else if (tipoCampo == 5044)// Depois de abrir a comunicação com o PinPad 
                                        { }
                                        else if (tipoCampo == 5049)
                                        {
                                            //if (ConsoleWrite) Console.WriteLine("Error timeout: {0}", mensagem);
                                            //GR000453      No es posible la conexión con el servidor del proveedor. ~1
                                            BuildMessage("GR000453", "Timeout");
                                        }
                                        else if (tipoCampo == 5074)//Debe firmarse el cupón
                                        {
                                            //if (ConsoleWrite) Console.WriteLine("Se requiere una firma: {0}", mensagem);
                                            if (mensagem == "1")
                                                respostaSitef = string.Format("{0}", mensagem);
                                        }
                        #endregion
                            break;
                        #endregion Case 0
                        case 1: //Mensagem para o visor do operador
                            //**CONECTANDO SITEF
                            if (tipoCampo == 5007 && !_inicializado)
                                SetReturnValueString(tipoCampo, mensagem);
                            //**TipoCampo==5008->Conectado ok
                            else if (tipoCampo == 5008)
                                SetReturnValueString(tipoCampo, mensagem);
                            //Código de sincronización Clover
                            //Códifo de sincronización cpara Clover
                            else if (tipoCampo == 5081)
                            {
                                //if (ConsoleWrite) Console.WriteLine("Código de emparejamiento: {0}", mensagem);
                                Regex r = new Regex(@"\d+");
                                Match m = r.Match(mensagem);

                                if (m.Success)
                                {
                                    _codemparejamiento = m.Value;
                                    SetReturnValueString(tipoCampo, _codemparejamiento);
                                }
                            }
                            else if (ConsoleWrite)
                            {
                                Console.WriteLine(mensagem);
                                SetReturnValueString(tipoCampo, mensagem);
                            }
                            break;
                        case 2: //Mensagem para o visor do cliente
                            if (ConsoleWrite)
                            {
                                Console.WriteLine(mensagem);
                                SetReturnValueString(tipoCampo, mensagem);
                            }
                            break;
                        case 3: //**Mensagem para os dois visores
                            //if (tipoCampo == -1)
                            //{
                            //    string _aux = "";
                            //    if (ConsoleWrite)
                            //    {
                            //        Console.WriteLine(mensagem);
                            //        _aux = mensagem.ToUpper();
                            //        if (!_aux.Contains("PROCESO"))
                            //            SetReturnValueString(tipoCampo, mensagem);
                            //    }
                            //}

                            if (ConsoleWrite)
                                Console.WriteLine(mensagem);

                            //**Leer tarjeta
                            if (tipoCampo == 5000)
                            {
                                SetReturnValueString(tipoCampo, mensagem);
                                if (_veces >= 3)
                                {
                                    //GR000451      No fue posible leer la tarjeta.
                                    continua = BuildMessage("GR000451", _while: _flag);

                                }
                                else if(_veces >0)
                                    Thread.Sleep(3000);
                                _veces += 1;
                            }
                            //**Transacción Autorizada
                            else if (tipoCampo == 5005)
                            {
                                SetReturnValueString(tipoCampo, mensagem);
                                //if(ConsoleWrite) Console.WriteLine("Transacción Autorizada. Mensaje Buffer: {0}", mensagem);
                            }
                            break;
                        case 4: //Texto que deverá ser utilizado como cabeçalho na apresentação do menu (Comando 21)
                            captionMenu = mensagem;
                            //if(ConsoleWrite) Console.WriteLine(mensagem);
                            break;
                        case 11: //Deve remover a mensagem apresentada no visor do operador
                            mensagem = "";
                            //if(ConsoleWrite) Console.WriteLine(mensagem);
                            break;
                        case 12: //Deve remover a mensagem apresentada no visor do cliente
                            mensagem = "";
                            //if(ConsoleWrite) Console.WriteLine(mensagem);
                            break;
                        case 13: //Deve remover mensagem apresentada no visor do operador e do cliente
                            mensagem = "";
                            //if(ConsoleWrite) Console.WriteLine(mensagem);
                            break;
                        case 14: //Deve limpar o texto utilizado como cabeçalho na apresentação do menu
                            captionMenu = "";
                            break;
                        case 15: //Cabeçalho a ser apresentado pela aplicação
                            captionMenu = mensagem;
                            break;
                        case 16: //Deve remover o cabeçalho
                            captionMenu = "";
                            break;
                        case 20: //Deve obter uma resposta do tipo SIM/NÃO. 0-> SI, 1->NO
                            if (string.IsNullOrWhiteSpace(mensagem))
                                mensagem += "Confirma?";
                            if (tipoCampo == 5013)//Confirmar operacion Anulada?
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = string.Format("{0}", "0");
                            }
                            //Error de lectura de tarjeta
                            else if (_ultimotipocampo == 5000)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = string.Format("{0}", _veces < 3 ? "0" : "1");
                                //GR000451      No fue posible leer la tarjeta.
                                //continua = BuildMessage("GR000451", _while: _flag);
                            }
                            else
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else //Automático
                                    respostaSitef = string.Format("{0}", "0");
                            }
                            if (ConsoleWrite)
                            {
                                Console.WriteLine("{0}. Tamñano Min: {1}, Tamaño Max: {2}", mensagem, tamanhoMinimo, tamanhoMaximo);
                                SetReturnValueString(tipoCampo, mensagem);
                            }
                            break;
                        case 21: //Deve apresentar um menu de opções e permitir que o usuário selecione uma delas. Na chamada o parâmetro Buffer contém as opções no formato 1:texto;2:texto;...i:Texto;... A rotina da aplicação deve apresentar as opções da forma que ela desejar (não sendo necessário incluir os índices 1,2, ...) e após a seleção feita pelo usuário, retornar em Buffer o índice i escolhido pelo operador (em ASCII)
                            if (ConsoleWrite)
                            {
                                Console.WriteLine(captionMenu + "\n" + mensagem);
                                SetReturnValueString(tipoCampo, captionMenu + "\n" + mensagem);
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                {
                                    //opciones de reimpresiones
                                    if((int)_tipodepago >= 56 && (int)_tipodepago <= 58)
                                    {
                                        respostaSitef = string.Format("{0}", "2"); 
                                    }
                                }
                            }
                            break;
                        case 22: //Deve aguardar uma tecla do operador. É utilizada quando se deseja que o operador seja avisado de alguma mensagem apresentada na tela
                            {
                                if (ConsoleWrite)
                                    Console.WriteLine(mensagem);
                                SetReturnValueString(tipoCampo, mensagem);
                                break;
                            }
                        case 23: //Este comando indica que a rotina está perguntando para a aplicação se ele deseja interromper o processo de coleta de dados ou não. Esse código ocorre quando a CliSiTef está acessando algum periférico e permite que a automação interrompa esse acesso (por exemplo: aguardando a passagem de um cartão pela leitora ou a digitação de senha pelo cliente)
                            if (ConsoleWrite)
                            {
                                Console.WriteLine("{0}. Tamñano Min: {1}, Tamaño Max: {2}", mensagem, tamanhoMinimo, tamanhoMaximo);
                                SetReturnValueString(tipoCampo, mensagem);
                            }
                            //respostaSitef =Console.ReadLine();
                            break;
                        case 29: //Deve ser fornecido um campo, sem captura, cujo tamanho está entre TamMinimo e TamMaximo. O campo deve ser devolvido em Buffer
                            if (ConsoleWrite) Console.WriteLine("campo que no requiera intervención del cajero. {0}", mensagem);
                            respostaSitef = Console.ReadLine();
                            break;
                        case 30: //Deve ser lido um campo cujo tamanho está entre TamMinimo e TamMaximo. O campo lido deve ser devolvido em Buffer
                            if (ConsoleWrite && ((tipoCampo == -1 && _ultimotipocampo != 30) || tipoCampo != -1))
                            {
                                if (_ultimomensaje.PadRight(10, ' ').Substring(0, 10) != mensagem.PadRight(10, ' ').Substring(0, 10))
                                {
                                    Console.WriteLine("{0}", mensagem);
                                    SetReturnValueString(tipoCampo, mensagem);
                                }

                            }
                            if (tipoCampo == 500)//Código de supervisor
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = _codsupervisor;
                            }
                            //**Cantidad de cuotas
                            else if (tipoCampo == 505)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = (_cuotas > 0) ? _cuotas.ToString() : "1";

                                SetReturnValueString(tipoCampo, mensagem + " " + respostaSitef);
                            }
                            //Tarjeta digitada
                            else if (tipoCampo == 512)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = string.Format("{0}", "4546400034748181");
                            }
                            //fecha de vencimiento
                            else if (tipoCampo == 513)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = string.Format("{0}", "1030");
                            }
                            else if (tipoCampo == 514)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = string.Format("{0}", "123");
                            }
                            else if (tipoCampo == 515)//Fecha de la transacción a devolver
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = _date;
                            }
                            //*Documento a devolver PinPad
                            else if (tipoCampo == 516)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = string.Format("{0:0000000000}", ResolverComprobante());

                                SetReturnValueString(tipoCampo, mensagem + " " + respostaSitef);
                            }
                            //**Plan de Cuotas
                            else if (tipoCampo == 519)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = string.Format("{0}", _plancuotas);

                                SetReturnValueString(tipoCampo, mensagem + " " + respostaSitef);
                            }
                            //Numero de Autorización NFCE
                            else if (tipoCampo == 952)
                            {
                                if (ConsoleWrite) Console.WriteLine("Numero de Autorización NFCE: {0}", mensagem);
                            }
                            /********************************************************/
                            /*                  Devolución Clover                   */
                            /********************************************************/
                            //PaymentId 
                            else if (tipoCampo == 4145)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = _paymentID;
                            }
                            //Order ID
                            else if (tipoCampo == 4146)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = _orderid;
                            }
                            /********************************************************/
                            //Se debe informa la IP:Puerto del Clover o su nombre Host 
                            else if (tipoCampo == 4160)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                    respostaSitef = _ipclover + ":" + _puerto.ToString();
                                Thread.Sleep(5000);
                            }
                            if (respostaSitef.Length == 0) respostaSitef = Console.ReadLine();
                            break;
                        case 31: //Deve ser lido o número de um cheque. A coleta pode ser feita via leitura de CMC-7 ou pela digitação da primeira linha do cheque. No retorno deve ser devolvido em Buffer “0:” ou “1:” seguido do número coletado manualmente ou pela leitura do CMC-7, respectivamente. Quando o número for coletado manualmente o formato é o seguinte: Compensação (3), Banco (3), Agencia (4), C1 (1), ContaCorrente (10), C2 (1), Numero do Cheque (6) e C3 (1), nesta ordem. Notar que estes campos são os que estão na parte superior de um cheque e na ordem apresentada. Sugerimos que na coleta seja apresentada uma interface que permita ao operador identificar e digitar adequadamente estas informações de forma que a consulta não seja feita com dados errados, retornando como bom um cheque com problemas
                            break;
                        case 34: //Deve ser leído um campo monetário ou seja, aceita o delimitador de centavos e devolvido no parâmetro Buffer
                            if (ConsoleWrite) Console.WriteLine(mensagem);
                            if (tipoCampo == 146)//Valor de la transacción/devolución
                            {
                                if (ConsoleMode)
                                {
                                    respostaSitef = Console.ReadLine();
                                    respostaSitef = respostaSitef.Replace('.', ',');
                                }
                                else
                                {
                                    respostaSitef = _import.ToString("F");
                                    respostaSitef = respostaSitef.Replace('.', ',');
                                    SetReturnValueString(tipoCampo, mensagem + ": " + respostaSitef);
                                }
                            }
                            //**Monto del vuelto
                            else if (tipoCampo == 130)
                            {
                                if (ConsoleMode)
                                    respostaSitef = Console.ReadLine();
                                else
                                {
                                    // "{0:N0}" : "{0:N2}"
                                    NumberFormatInfo nfi = new NumberFormatInfo();
                                    nfi.NumberDecimalSeparator = ",";
                                    if (!string.IsNullOrWhiteSpace(respostaSitef) && Convert.ToDecimal(respostaSitef) > 0)
                                    {
                                        decimal d = Convert.ToDecimal(respostaSitef);
                                        respostaSitef = d.ToString("#.00").Replace('.', ',');
                                    }
                                    else
                                    {
                                        decimal d = 0.00M;
                                        respostaSitef = d.ToString(nfi);
                                    }
                                }
                                SetReturnValueString(tipoCampo, mensagem + "|" + respostaSitef);
                            }

                            break;
                        case 35: //Deve ser lido um código em barras ou o mesmo deve ser coletado manualmente. No retorno Buffer deve conter “0:” ou “1:” seguido do código em barras coletado manualmente ou pela leitora, respectivamente. Cabe ao aplicativo decidir se a coleta será manual ou através de uma leitora. Caso seja coleta manual, recomenda-se seguir o procedimento descrito na rotina ValidaCampoCodigoEmBarras de forma a tratar um código em barras da forma mais genérica possível, deixando o aplicativo de automação independente de futuras alterações que possam surgir nos formatos em barras. No retorno do Buffer também pode ser passado “2:”, indicando que a coleta foi cancelada, porém o fluxo não será interrompido, logo no caso de pagamentos múltiplos, todos os documentados coletados anteriormente serão mantidos e o fluxo retomado, permitindo a efetivação de tais pagamentos.
                            break;
                        case 41: //Análogo ao Comando 30 (TextInputNeeded), porém o campo deve ser coletado de forma mascarada (senha).
                            if(ConsoleWrite) Console.WriteLine(mensagem);
                            respostaSitef = Console.ReadLine();

                            break;
                        case 42: //Deve apresentar um menu de opções e permitir que o usuário selecione uma delas.
                            if (ConsoleWrite)
                            {
                                Console.WriteLine(mensagem);
                                SetReturnValueString(tipoCampo, mensagem);
                            }
                            break;
                        case 50: //A automação comercial deve exibir o QRCode na tela. Para tanto, neste mesmo comando, será devolvida a string do QRCode com a identificação de campo 584.
                            if(ConsoleWrite) Console.WriteLine(mensagem);
                            break;
                        case 51: //A automação comercial deve remover da tela o QRCode exibido anteriormente, pois o SiTef já devolveu uma resposta à CliSiTef.
                            if(ConsoleWrite) Console.WriteLine(mensagem);
                            captionCarteiraDigital = "";
                            break;
                        case 52: //Mensagem de rodapé, opcional para o caso haja um espaço para ela ser exibida, no caso em que o QRCode foi exibido e está aguardando que o cliente faça a sua leitura.
                            if(ConsoleWrite) Console.WriteLine(mensagem);
                            break;
                        case 99:
                            break;

                        default:
                            break;
                    }
                    if (respostaSitef == null)
                        respostaSitef = "";
                    valorBuffer = Encoding.ASCII.GetBytes(respostaSitef + new string('\0', 20000 - respostaSitef.Length));
                }
                else if (result == 0)
                {
                    if(ConsoleWrite) Console.WriteLine("Bucle finalizado satisfactoriamente. Mensaje Sitef: {0}", mensagem);
                }
                else if (result != 0 && result != 10000)
                {
                    /*
                        0 Continuar la transacción
                        1  Volver a recolectar el campo anterior, si es posible.
                        2 Cancelar
                        -1 Terminar Transacción
                    */

                            if (ConsoleWrite) Console.WriteLine("Error: {0}, result: {1}" , mensagem, result);
                    if (_continua != _flag && _errorlogcode.Length == 0)
                    {
                        switch (result)
                        {
                            case -2:
                            case -6:
                                //GR000363 El POS respondió: Transacción cancelada por el usuario
                                BuildMessage("GR000363", mensagem);
                                break;
                            //Parámetro Función/Function no es válido
                            case -3:
                                //GR000068      ~1
                                BuildMessage("GR000068", "El Parámetro Función no es válido");
                                break;
                            //No es posible la comunicación con SiTef
                            case -5:
                                //GR000453      No es posible la conexión con el servidor del proveedor
                                BuildMessage("GR000453");
                                break;
                            case -8:
                                //GR000464      El POS respondió: CliSiTef no tiene implementada la función solicitada; probablemente está desactualizado
                                BuildMessage("GR000464");
                                break;
                            case -12:
                                //GR000454      El POS no respondió o existe un error interno en una de sus rutinas. ~1
                                continua = BuildMessage("GR000454", mensagem);
                                if (ConsoleWrite) Console.WriteLine("Error: {0},{1}", _errorlogcode, _errorlogdescrp);
                                break;
                            case -15:
                                //GR000363      El POS respondió: Transacción cancelada por el usuario
                                continua = BuildMessage("GR000363", mensagem);
                                if (ConsoleWrite) Console.WriteLine("Error: {0},{1}", _errorlogcode, _errorlogdescrp);
                                break;
                            case -20:
                                //GR000370      El POS respondió: Formato de algún parámetro de entrada no es correcto
                                continua = BuildMessage("GR000370", mensagem);
                                if (ConsoleWrite) Console.WriteLine("Error: {0},{1}", _errorlogcode, _errorlogdescrp);
                                break;
                            //Error de lectura de archivos
                            case -30:
                                //GR000462      El POS respondió: error de acceso a archivos requeridos por la aplicación
                                continua = BuildMessage("GR000462", mensagem);
                                if (ConsoleWrite) Console.WriteLine("Error: {0},{1}", _errorlogcode, _errorlogdescrp);
                                break;
                            //Transacción denegaga por Sitef
                            case -40:
                                //GR000461      Transacción denegada por el proveedor
                                continua = BuildMessage("GR000461", mensagem);
                                if (ConsoleWrite) Console.WriteLine("Error: {0},{1}", _errorlogcode, _errorlogdescrp);
                                break;
                            //Error en una rutina dentro del PinPad
                            case -43:
                            case -801:
                                //GR000454      El POS no respondió o existe un error interno en una de sus rutinas. ~1
                                continua = BuildMessage("GR000454", mensagem);
                                if (ConsoleWrite) Console.WriteLine("Error: {0},{1}", _errorlogcode, _errorlogdescrp);
                                break;
                            case -100:
                                //GR000465      El POS respondió: Error interno CliSiTef. Si posee un Clover, realice nuevamente la sincronización
                                continua = BuildMessage("GR000454");
                                if (ConsoleWrite) Console.WriteLine("Error: {0},{1}", _errorlogcode, _errorlogdescrp);
                                break;
                            default:
                                //GR000068      ~1
                                BuildMessage("GR000068", mensagem);
                                if(ConsoleWrite) Console.WriteLine("Error: {0},{1}", _errorlogcode, _errorlogdescrp);
                                break;
                        }
                    }
                }
                _ultimotipocampo = tipoCampo;
                _ultimomensaje = mensagem;
            } while (result == 10000);
            datosimprimir = aux;
            return result;
        }

        int BuildMessage(string _code, string _param = "", int _while = 0)
        {
            _errorlogcode = _code;
            _errorlogdescrp = string.Format("{0}",_param);
            return _while;
        }
           
        string RestriccionesMediosDePago(TipoTarjeta tarjeta, TipoFormaPago forma, short cuotas)
        {
            StringBuilder cadena = new StringBuilder();
            string _valor;
            bool _devolucion = _tipooperacion == TipoOperacion.Devolucion ? true : false;
            cadena.Append("[");
            /*
             Clave de débito ingresada 42
             Débito magnético 43
            ------------------------------
            Tarjeta de crédito ingresada con clave 29
            Tarjeta de crédito magnética 30
            */
            switch (tarjeta)
            {
                case TipoTarjeta.TCreditoDIG:
                    cadena.Append("30;");
                    if (_devolucion) _funcion = TipoFuncion.DevolucionTCDIG;
                    break;
                case TipoTarjeta.ClaveDebitoDIG:
                    cadena.Append("43;");
                    if (_devolucion) _funcion = TipoFuncion.DevolucionGral;
                    break;
                case TipoTarjeta.TCreditoMAG:
                    cadena.Append("29;");
                    if (_devolucion) _funcion = TipoFuncion.DevolucionTCMAG;
                    break;
                case TipoTarjeta.TDebitoMAG:
                    cadena.Append("42;");
                    if (_devolucion) _funcion = TipoFuncion.DevolucionTDMAG;
                    break;
            }

            if (_funcion == TipoFuncion.Credito || _funcion == TipoFuncion.Debito || _funcion == TipoFuncion.PGeneral)
            {
                //Forma de pago (cuotas, efectivo, etc)
                /*
                    Tarjeta de débito "en efectivo" (sin cuotas) 16
                    Tarjeta de débito con cuotas 18
                    ----------------------------------------
                    Tarjeta de crédito "en efectivo" con intereses 24
                    Tarjeta de crédito (todas las combinaciones) 25
                    Tarjeta de crédito "en efectivo" 26
                    Tarjeta de crédito con reembolsos financiados por el comerciante 27
                    Tarjeta de crédito con reembolsos financiados por el emisor de la tarjeta 28
                */
                switch (forma)
                {
                    case TipoFormaPago.TCreditoEFT:
                        //Si se asignó a la función Crédito, solo se restringe las opciones de crédito. 24;26 es TC Efectivo
                        cadena.Append("24;27;28;");
                        break;
                    //TC en efectivo con Interés
                    case TipoFormaPago.TCreditoEFTCI:
                        cadena.Append("26;27;28;");
                        break;
                    case TipoFormaPago.TDebitoEFT:
                        cadena.Append("18;");
                        break;
                    case TipoFormaPago.TcreditoTodas:
                        //Parte->CL-40497
                        //cadena.Append("24;26;27;");
                        cadena.Append("24;26;");
                        //
                        break;
                    case TipoFormaPago.TDebitoTodas:
                        cadena.Append("16;");
                        break;
                }
                cadena.Append("3031;3582;4151;");
            }   
            else if(_funcion == TipoFuncion.Gerencial)
            {
                cadena.Append("40;15;16;18;24;27;28;4151;");
                /*switch (forma)
                {
                    case TipoFormaPago.Reimprimir:
                        cadena.Append("57;58;");
                        break;
                    case TipoFormaPago.ReimprimirUltimo:
                        cadena.Append("56;58;");
                        break;
                    case TipoFormaPago.ReimprimirConID:
                        cadena.Append("56;57;");
                        break;
                }
                */
            }
            else if(_tipooperacion == TipoOperacion.Devolucion)
            {
                //'#Parte->CL-40497->
                if (_tipopinpad == TipoPinPad.Clover)
                {
                    _funcion = TipoFuncion.DevolucionClover;
                }
            }

            cadena.Append("]");
            _valor = cadena.ToString();
            return _valor;
        }
        void Inicializar(TipoInicializacion ti)
        {
            switch (ti)
            {
                case TipoInicializacion.Construct:

                    _errorlogcode       = string.Empty;
                    _errorlogdescrp     = string.Empty;
                    _cupon              = string.Empty;
                    _codautorizacion    = string.Empty;
                    _idtransaccion      = string.Empty;
                    _ipsitef            = string.Empty;
                    _ipclover           = string.Empty;
                    _cuit               = string.Empty;
                    _cuitisv            = _cuitdeveloper;
                    _comercio_id        = string.Empty;
                    _terminal_id        = string.Empty;
                    _factura            = string.Empty;
                    _codsupervisor      = string.Empty;
                    _nsusitef           = string.Empty;
                    _cuponorig          = string.Empty;
                    _cuponcopia         = string.Empty;
                    _date               = string.Empty;
                    _primerosdigitos    = string.Empty;
                    _ultimosdigitos     = string.Empty;
                    _orderid            = string.Empty;
                    _paymentID          = string.Empty;
                    _args               = string.Empty;
                    _listcancelpending  = string.Empty;
                    _listDelimiter      = ";";
                    _fieldsDelimiter    = "#";
                    _nexoDelimiter      = "=";
                    _cancelpending      = false;
                    Imprimir            = false;
                    _fecha              = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture);
                    _hour               = DateTime.Now.ToString("HHmmss");
                    _inicializado       = false;
                    ConsoleMode         = false;
                    _tiempoesperacopia  = 0;
                    _cuotas             = 0;
                    _plancuotas         = 0;
                    _transpending       = 0;
                    _ts                 = TimeSpan.FromSeconds(180);
                    _stkeyvalue = new StructKeyValue() { cadena = "", splitchar = "" };
                    Utilities.DateString(_fecha, CultureInfo.InvariantCulture, TypeDate.ja_JP, out _date);
                    _dateini        = _date;
                    break;
                case TipoInicializacion.ConfigurationSitef:
                    _ipsitef        = string.Empty;
                    _ipclover       = string.Empty;
                    _cuit           = string.Empty;
                    _cuitisv        = _cuitdeveloper;
                    _comercio_id    = string.Empty;
                    _terminal_id    = string.Empty;
                    break;
                case TipoInicializacion.ConfigurationIni:
                    _funcion        = TipoFuncion.PGeneral;
                    _tipooperacion  = TipoOperacion.Ninguno;
                    _tipopinpad     = TipoPinPad.Ninguno;
                    _puerto         = 0;
                    _ipclover       = string.Empty;
                    break;
                case TipoInicializacion.Transaction:
                    _factura            = string.Empty;
                    _fecha              = string.Empty;
                    _hour               = string.Empty;
                    _operador           = string.Empty;
                    _errorlogcode       = string.Empty;
                    _errorlogdescrp     = string.Empty;
                    _cupon              = string.Empty;
                    _codautorizacion    = string.Empty;
                    _idtransaccion      = string.Empty;
                    _cuponorig          = string.Empty;
                    _cuponcopia         = string.Empty;
                    _primerosdigitos    = string.Empty;
                    _ultimosdigitos     = string.Empty;
                    _tipodepago         = TipoFormaPago.Niguno;
                    _tipotarjeta        = TipoTarjeta.Niguno;
                    _codemptarjeta      = EmpTarjeta.Otro;
                    _codtarjeta         = EmpTarjeta.Otro;
                    _import             = 0;
                    _cuotas             = 0;
                    _ts = TimeSpan.FromSeconds(_timeoutsec);
                    break;
                case TipoInicializacion.Finish:
                    _ipsitef            = string.Empty;
                    _ipclover           = string.Empty;
                    _cuit               = string.Empty;
                    _comercio_id        = string.Empty;
                    _terminal_id        = string.Empty;
                    _factura            = string.Empty;
                    _codsupervisor      = string.Empty;
                    _cuponcopia         = string.Empty;
                    _cuponorig          = string.Empty;
                    _listcancelpending = string.Empty;
                    _cancelpending      = false;
                    Imprimir            = false;
                    _tiempoesperacopia  = 0;
                    _cuotas             = 0;
                    _plancuotas         = 0;
                    break;
            }
        }
        bool Validaciones(TipoInicializacion ti)
        {
            bool _valida = true;
            string formatDate = string.Empty;
            switch (ti)
            {
                case TipoInicializacion.ConfigurationSitef:
                    if (_cuit.Length == 0 || _comercio_id.Length == 0 || _terminal_id.Length == 0)
                    {
                        _valida = false;
                        //GR000196      Debe completar el campo ~1
                        BuildMessage("GR000196", "CUIT/COMERCIO_ID/TERMINAL_ID");
                    }
                    else if (!IPAddress.TryParse(_ipsitef, out IPAddress _))
                    {
                        _valida = false;
                        //GR000069      El campo ~1 Tiene un valor inválido
                        BuildMessage("GR000069", "Dirección de IP del proveedor");
                    }
                    else if (_cuitisv.Length == 0)
                        _cuitisv = _cuitdeveloper;
                    break;
                //BeginTransaction
                case TipoInicializacion.Transaction:
                    if (_tipooperacion == TipoOperacion.Ninguno)
                    {
                        _valida = false;
                        //GR000196      Debe completar el campo ~1
                        BuildMessage("GR000196", "Tipo de operación");
                    }

                    else if (_factura.Length == 0)
                    {
                        _valida = false;
                        BuildMessage("GR000196", "Número de comprobante");
                    }
                    else if (_import == 0)
                    {
                        _valida = false;
                        BuildMessage("GR000196", "Importe");
                    }
                    else if (_tipotarjeta == TipoTarjeta.Niguno)
                    {
                        _valida = false;
                        BuildMessage("GR000196", "Tipo de tarjeta");
                    }
                    else if (_tipotarjeta != TipoTarjeta.TCreditoDIG && _tipotarjeta != TipoTarjeta.ClaveDebitoDIG)
                    {
                        if (_tipopinpad == TipoPinPad.Ninguno)
                        {
                            _valida = false;
                            //GR000455      El tipo de POS envíado no se corresponde con la operación (~1)
                            BuildMessage("GR000455", TipoPinPad.Ninguno.ToString());
                        }
                        else if (_tipopinpad == TipoPinPad.Clover)
                        {
                            if (!IPAddress.TryParse(_ipclover, out _) || _puerto <=0)
                            {
                                BuildMessage("GR000196", "IP Clover o Puerto HTTP.\nParámetros:" + _ipclover + ", " + _puerto.ToString());
                                _valida = false;
                            }
                        }
                        else if(_tipopinpad == TipoPinPad.PinPad && _puerto == 0)
                        {
                            _valida = false;
                            //GR000069      El campo ~1 Tiene un valor inválido
                            BuildMessage("GR000069", "pueto COM");
                        }
                    }
                    else if (_funcion == TipoFuncion.PGeneral || _funcion == TipoFuncion.Credito || _funcion == TipoFuncion.Debito)
                    {
                        if (_tipodepago == TipoFormaPago.Niguno)
                        {
                            _valida = false;
                            BuildMessage("GR000196", "Tipo de pago");
                        }
                        else if (_tipodepago == TipoFormaPago.TCreditoFinaciaComercio || _tipodepago == TipoFormaPago.TCreditoFinanciaTarjeta || _tipodepago == TipoFormaPago.TcreditoTodas)
                        {
                            if (_cuotas == 0)
                            {
                                _valida = false;
                                BuildMessage("GR000196", "Cuotas");
                            }

                        }
                    }

                    if (_tipooperacion == TipoOperacion.Devolucion)
                    {
                        if (_codsupervisor.Length == 0)
                        {
                            _valida = false;
                            BuildMessage("GR000196", "Código de supervisor");
                        }
                        else if (_nsusitef.Length == 0)
                        {
                            _valida = false;
                            //GR000069      El campo ~1 Tiene un valor inválido
                            BuildMessage("GR000196", "Código NSU");
                        }
                        else if (_tipopinpad == TipoPinPad.Clover &&(_paymentID.Length == 0 || _orderid.Length == 0))
                        {
                            _valida = false;
                            BuildMessage("GR000196", "PaymentID/OrderID");
                        }
                        else
                        //Fecha
                        {
                        //'#Parte->CL-40497->
                            //if (!Utilities.DateValidate(_date, TypeDate.fr_FR))
                            //{
                            //    _valida = false;
                            //    //GR000069      El campo ~1 Tiene un valor inválido
                            //    BuildMessage("GR000196", "Fecha. Fecha recibida: " + _fecha + ", Convertida: " + _date + ". Se requiere: AAAAMMDD");
                            //}
                            if (_funcion == TipoFuncion.DevolucionClover)
                            {
                                if (!Utilities.DateValidate(_date, TypeDate.ja_JP))
                                {
                                    formatDate = "AAAAMMDD";
                                    _valida = false;
                                }
                            }
                            else if (!Utilities.DateValidate(_date, TypeDate.fr_FR))
                            {
                                formatDate = "DDMMAAAA";
                                _valida = false;
                            }
                            if(!_valida)
                            {
                                //GR000069      El campo ~1 Tiene un valor inválido
                                BuildMessage("GR000196", "Fecha. Fecha recibida: " + _fecha + ", Convertida: " + _date + ". Se requiere: " + formatDate);
                            }

                        //'#Parte->CL-40497<-
                        }
                    }
                    else
                        if (!Utilities.DateValidate(_date, TypeDate.ja_JP))
                        {
                            _valida = false;
                            //GR000069      El campo ~1 Tiene un valor inválido
                            BuildMessage("GR000196", "Fecha recibida: " + _fecha + "\nConvertida: " + _date + "\nFormato de conversión(AAAAMMDD)");
                        }
                    break;
            }
            return _valida;
        }

        //Imprimir cupones
        async Task<int> ImprimirCupon(string nrocupon, string cuponorig, string cuponcopia, int tmpsleep = 0)
        {
            await Task.Run(() =>
            {
                if (cuponorig.Length > 0)
                    if (!(ImprimePOS(cuponorig, "") == 0))
                    {
                        //GR000368      El POS respondió: El POS no pudo imprimir el ticket ~1
                        BuildMessage("GR000368", "Número de cupón: " + String.Format("{0}", nrocupon), _flag);
                        _true = false;
                    }
                    else if (cuponcopia.Length > 0)
                    {
                        //El tmpsleep viene es segundos, entoces se pasa a ms
                        Task.Delay(tmpsleep*1000);
                        if (!(ImprimePOS(cuponcopia, "") == 0))
                        {
                            //GR000368      El POS respondió: El POS no pudo imprimir el ticket ~1
                            BuildMessage("GR000368", "Número de cupón: " + String.Format("{0}", nrocupon), _flag);
                            _true = false;
                        }
                    }
            });
            return _true ? 0 : 1;
        }
        string ResolverComprobante()
        {
            string _comprobante = "";
            switch (_funcion )
            {
                case TipoFuncion.DevolucionGral:
                case TipoFuncion.DevolucionPreAuto:
                case TipoFuncion.DevolucionTCDIG:
                case TipoFuncion.DevolucionTCMAG:
                case TipoFuncion.DevolucionTDMAG:
                    _comprobante = _nsusitef;
                    break;
                default:
                    _comprobante = _factura;
                    break;
            }
            return _comprobante;
        }
        void ConfigClisitefIni()
        {
            if (_tipopinpad == TipoPinPad.Clover)
            {
                Utilities.IniWriteValue(_inifile,"PinPadCompartilhado", "TipoPinPadComp", "CLOVERTEF");
                Utilities.IniWriteValue(_inifile,"PinPadCompartilhado", "Porta", null);
            }
            else if (_tipopinpad == TipoPinPad.PinPad)
            {
                Utilities.IniWriteValue(_inifile,"PinPadCompartilhado", "TipoPinPadComp", "POSNET");
                Utilities.IniWriteValue(_inifile,"PinPadCompartilhado", "Porta", _puerto.ToString());
                //Elimina la sección y sus claves null, null
                Utilities.IniWriteValue(_inifile,"CloverDevice", null, null);
                
            }

            else return;
        }
        protected virtual void GetKeyValue(StructKeyValue st)
        {
            try
            {
                Dictionary<string, string> dic =
                    st.cadena.Split(new[] { st.splitchar }, StringSplitOptions.None)
                    .Select(x => x.Split('=')).Where(y => !string.IsNullOrWhiteSpace(y[0]))
                    .GroupBy(x => x[0].ToUpper(), x => x[1])
                    .ToDictionary(x => x.Key, x => x.First());
                if (dic.ContainsKey("ORDERID"))
                    _orderid = dic["ORDERID"];
                if (dic.ContainsKey("PAYMENTID"))
                    _paymentID = dic["PAYMENTID"];
                //if (dic.ContainsKey("FCHMOV"))
                //    _fecha = dic["FCHMOV"];
            }
            catch (Exception e)
            { if(ConsoleWrite) Console.WriteLine(e.Message); }
        }
        protected void FinishListTasks()
        {
            _tasks = ListTasks();
            Task t = Task.WhenAll(_tasks);
            try
            {
                t.Wait();
            }
           catch { }
           finally
           {
            _tasks = null;
            t.Dispose();

            }
        }
        protected void SetReturnValueString(int TipoCampo, string Message)
        {
            if (ReturnValue != null)
                ReturnValue.ReturnStringValue((Int32)TipoCampo, Message, ref _yesnot);
            Thread.Sleep(1000);
        }
        protected bool IsPrint(string cuponcopia, string cuponorig, bool imprimir, TipoFormaPago tfpago)
        {
            bool _print = false;
            switch(tfpago)
            {
                case TipoFormaPago.Reimprimir:
                case TipoFormaPago.ReimprimirUltimo:
                case TipoFormaPago.ReimprimirConID:
                    _print = true;
                    break;
                default:
                    _print = false;
                    break;
            }

            return (cuponcopia != null && cuponorig != null && imprimir) ||(cuponorig != null && imprimir && _print);
        }

        protected bool FinalizaFuncion(TipoFuncion f)
        {
            bool _r;
            switch(f)
            {
                case TipoFuncion.PendientesTodos:
                    _r = false;
                    break;
                default:
                    _r = true;
                    break;

            }
            return _r;
        }
        protected int FinalizarComprobantes(bool boollist, string liststring, string listdelimitador, string fieldsdelimiter)
        {
            List<TransPending> l = Utilities.StringToListClass(liststring, listdelimitador, fieldsdelimiter);
            int _r = _flag;
            foreach (TransPending item in l)
            {
                if (Utilities.DateValidate(item.Date, TypeDate.ja_JP))
                {
                    _r = FinalizaFuncaoSiTefInterativoEx((short)(boollist ? 1 : 0), item.Comprobante, (string)item.Date, (string)_hour, "");
                }
                else
                    //GR000069      El campo ~1 Tiene un valor inválido
                    _r = BuildMessage("GR000069", _param: "Fecha del comprobante:" + item.Comprobante.ToString());

                if (_r != 0) break;
            }

            return _r;
        }

        protected bool IsSale(TipoFuncion tp)
        {
            bool _true = false;
            switch(tp)
            {
                case TipoFuncion.CargaTablaPinPad:
                case TipoFuncion.ConfigClover:
                case TipoFuncion.MenuReimpresion:
                case TipoFuncion.PendienteID:
                case TipoFuncion.PendientesTodos:
                case TipoFuncion.ReimpresionConID:
                case TipoFuncion.ReimpresionUltimo:
                case TipoFuncion.Gerencial:
                    _true = false;
                    break;
                default:
                    _true = true;
                    break;
            }
            return _true;
        }
        protected int ValidarTarjeta(string msg)
        {
            int _continua = 0;
            string msgAut = msg.PadRight(4, '0');
            string _grupo = msgAut.Substring(0, 2);
            string _subgrupo = msgAut.Substring(2, 2);

            switch (_grupo)
            {
                //Tarjeta de débito
                case "01":
                    if (_tipotarjeta == TipoTarjeta.TCreditoMAG)
                    {
                        //GR000364 El POS respondió: La tarjeta deslizada por el usuario no coincide con la pedida (Se envió ~1)
                        _continua = BuildMessage("GR000364", _codemptarjeta.ToString(), _flag);
                    }
                    break;
            }
            /*
                * 00
                À vista
                01
                Pré-datado
                02
                Parcelado com financiamento pelo estabelecimento
                03
                Parcelado com financiamento pela administradora
                99
                Outro tipo de pagamento
            */
            switch (_subgrupo)
            {
                //A Vista
                case "00":

                    break;
                case "01":

                    break;

                case "99":

                    break;
            }
            return _continua;
        }
    }
    


        #endregion
}
