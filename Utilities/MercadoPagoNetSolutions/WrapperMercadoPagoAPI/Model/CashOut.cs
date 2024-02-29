using System.ComponentModel;

namespace WrapperMercadoPagoAPI.Model;
public class CashOut
{
    [DefaultValue(-1)]
    /// <summary>
    /// Opcional: Retiro de efectivo
    /// </summary>
    public double amount { get; set; }
}
