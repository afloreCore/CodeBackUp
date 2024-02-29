using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WrapperMercadoPagoAPI.Model;
public class Sucursal
{
    public Sucursal()
    {
    }
    [DefaultValue(0)]
    public long id { get; set; }

    [Required, DefaultValue("")]
    public string name { get; set; } = string.Empty;
    [Required, DefaultValue("")]
    public string external_id { get; set; } = string.Empty;
    //public Business_Hours business_hours { get; set; } = null!;
    [DefaultValue(null)]
    public object? business_hours { get; set; } = null!;
    [DefaultValue(null)]
    public Location location { get; set; } = null!;
    public string date_creation { get; set; } = string.Empty;
}

