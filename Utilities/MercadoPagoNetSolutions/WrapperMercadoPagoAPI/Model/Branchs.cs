namespace WrapperMercadoPagoAPI.Model;
public class Branchs
{
    public Paging paging { get; set; } = null!;
    public List<BranchRequest> results { get; set; } = null!;

}
