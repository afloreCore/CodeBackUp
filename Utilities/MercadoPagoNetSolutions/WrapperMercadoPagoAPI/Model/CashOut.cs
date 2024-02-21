using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperMercadoPagoAPI.Model;
public class CashOut
{
    [DefaultValue(-1)]   
        /// <summary>
        /// Opcional: Retiro de efectivo
        /// </summary>
        public double amount { get; set; }
}
