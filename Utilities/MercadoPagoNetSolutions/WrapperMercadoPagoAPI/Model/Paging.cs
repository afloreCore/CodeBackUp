using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperMercadoPagoAPI.Model;

public class Paging
{
    public int total { get; set; }
    public int offset { get; set; }
    public int limit { get; set; }

}

