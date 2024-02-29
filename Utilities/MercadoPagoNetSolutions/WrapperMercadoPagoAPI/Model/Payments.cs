namespace WrapperMercadoPagoAPI.Model;
public class Payments
{
    public Paging paging { get; set; } = null!;
    public List<PaymentRequest> results { get; set; } = null!;
}
