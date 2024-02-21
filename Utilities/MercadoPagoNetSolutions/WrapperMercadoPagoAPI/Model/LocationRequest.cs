using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperMercadoPagoAPI.Model;
public class LocationRequest
{
    public string address_line { get; set; } = string.Empty;
    public string reference { get; set; } = string.Empty;
    public double latitude { get; set; }
    public double longitude { get; set; }
    public string id { get; set; } = string.Empty;
    public string type { get; set; } = string.Empty;
    public string city { get; set; } = string.Empty;
}
