namespace WrapperMercadoPagoAPI.Model;
public class Sucursales
{
    public Paging paging { get; set; } = null!;
    public List<SucursalRequest> results { get; set; } = null!;
}
