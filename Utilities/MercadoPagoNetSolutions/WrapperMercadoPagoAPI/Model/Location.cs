namespace WrapperMercadoPagoAPI.Model;
public partial class Location
{
    public string street_number { get; set; } = string.Empty;
    public string street_name { get; set; } = string.Empty;
    public string city_name { get; set; } = string.Empty;
    public string state_name { get; set; } = string.Empty;
    public string reference { get; set; } = string.Empty;
    public double latitude { get; set; } = -32.8897322;
    public double longitude { get; set; } = -68.8443275;

}

