using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrwPinPadNet
{
    public class Main2Program
    {
        public void main()
        {
            var t = DateTime.TryParse(DateTime.Now.Date.ToString(), CultureInfo.CurrentCulture, DateTimeStyles.None, out _);
            var r = DateTime.TryParse("23224555", CultureInfo.CreateSpecificCulture("fr-FR"), DateTimeStyles.None, out _);
            string date = ""; // DateTime.Now.Date.ToString("yyyyMMdd", CultureInfo.CreateSpecificCulture("fr-FR"));
            string _terminal = "AR000001";
            TipoPinPad tp = TipoPinPad.Clover;
            int _port = 9000;
            //tfdev = TipoFuncion.DevolucionTCMAG;
            EmpTarjeta emptj = EmpTarjeta.Amex;
           // string _admin = "ADMIN";
            string _comercio = "ARGSFL00";
            PinPad p = new PinPad();
            p.TimeOutSec = 360;
            p.CodSupervisor = "1234";
            p.ConfigSiteSitef("52.67.141.229", _terminal, _comercio, "30646941136");// ERROR_DE_COMANDO
            
            // 190.192.169.210
            bool config = false;

            if (config)
            {
                p.ConfigTransaction(TipoFuncion.ConfigClover, TipoOperacion.General, TipoPinPad.Clover, _port, "192.168.0.89");
                p.Begin("3444", 1, TipoFormaPago.TcreditoTodas, TipoTarjeta.TCreditoDIG, emptj, DateTime.Now.Date.ToShortDateString(), NroDeCuotas: 1, Usuario: "ADMIN");
            }
            p.TimeOutSec = 360;
            double import = 50;
            short Cuotas = 1;
            p.Imprimir = false;
            p.TiempoEsperaCopia = 0;
            p.DebugPrint = true;
            p.ConsoleWrite = true;
            p.ConsoleMode = true;
            p.CodSupervisor = "1234";
            p.CodOperador = "ADMIN";
            p.ConfigTransaction(TipoFuncion.PGeneral, TipoOperacion.General, tp, _port, "192.168.0.89");
            //Si es debito deber ir DebitoEFT
            p.Begin("22", import, TipoFormaPago.TcreditoTodas, TipoTarjeta.TCreditoMAG, emptj, "", NroDeCuotas: Cuotas, Usuario:"ADMIN");
            p.GetLastTrPending(";","#");
            var tr = p.CompLastTrPending();
        }
    }
}
