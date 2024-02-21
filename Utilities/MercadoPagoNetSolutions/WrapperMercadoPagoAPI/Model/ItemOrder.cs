using System.ComponentModel.DataAnnotations;
using WrapperMercadoPagoAPI.General;

namespace WrapperMercadoPagoAPI.Model;
public class ItemOrder
{
    /// <summary>
    /// Cógigo o identificación del item, puede no estar
    /// </summary>
    public string sku_number { get; set; } = string.Empty;
    /// <summary>
    /// Puede no estar
    /// </summary>
    public string category { get; set; } = string.Empty;
    /// <summary>
    /// Puede estar vacío
    /// </summary>
    public string title { get; set; } = string.Empty;
    /// <summary>
    /// puede no estar
    /// </summary>
    public string description { get; set; } = string.Empty;
    [Required]
    public double unit_price { get; set; }
    [Required]
    public double quantity { get; set; }
    /// <summary>
    /// puede ser vacío
    /// </summary>
    public string unit_measure { get; set; } = string.Empty;
    [Required]
    /// <summary>
    /// Suma total del item. (no se controla: precioxcantid)
    /// </summary>
    public double total_amount { get; set; }
}

