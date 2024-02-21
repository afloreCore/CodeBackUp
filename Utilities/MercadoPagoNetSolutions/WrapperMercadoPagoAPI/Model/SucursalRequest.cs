using System.Collections;

namespace WrapperMercadoPagoAPI.Model;
public class SucursalRequest
{

    public string id { get; set; } = string.Empty;
    public string name { get; set; } = null!;
    public string date_creation { get; set; } = string.Empty;
    public object business_hours { get; set; } = null!;
    public LocationRequest location { get; set; } = null!;
    public string external_id { get; set; } = null!;

}

