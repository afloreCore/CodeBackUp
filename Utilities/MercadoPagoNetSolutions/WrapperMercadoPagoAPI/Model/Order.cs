using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WrapperMercadoPagoAPI.General;

namespace WrapperMercadoPagoAPI.Model;


public class Order: IDisposable
{
    private bool _disposed;
    [DefaultValue("")]
    /// <summary>
    /// Referncia del comprobante en el PDV. Puede estar vacía
    /// </summary>
    public string external_reference { get; set; } = string.Empty;
    [DefaultValue("")]
    public string status { get; set; } = string.Empty;
    [DefaultValue(" ")]
    /// <summary>
    /// Puede estar vacío
    /// </summary>
    public string title { get; set; } = string.Empty;
    [Required ,DefaultValue(0)]
    /// <summary>
    /// Suma de los total_amount de los items + retiro de efectivo
    /// </summary>
    public double total_amount { get; set; }
    [DefaultValue(" ")]
    /// <summary>
    /// Puede estar vacío
    /// </summary>
    public string description { get; set; } = string.Empty;

    [DefaultValue("")]
    public string expiration_date { get; set; } = string.Empty;

    [DefaultValue("")]
    public string notification_url { get; set; } = string.Empty;
    public List<ItemOrder> items { get; set; } = null!;

    [DefaultValue(null)]
    public CashOut cash_out { get; set; } = null!;

    [DefaultValue(null)]    
    public Sponsor sponsor { get; set; } = null!;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                items.Clear();
                items = null!;
                cash_out = null!;
            }
            _disposed = true;
        }
    }
}
