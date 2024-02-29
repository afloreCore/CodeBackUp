using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.MarshalModel;

[ComVisible(true), Guid("F7E34492-9632-47CB-9974-13EDCFD7D3EC"), ClassInterface(ClassInterfaceType.AutoDispatch), ProgId("ItemOrderMarshal")]
public class ItemOrderMarshal
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
    public double unit_price { get; set; }
    public double quantity { get; set; }
    /// <summary>
    /// puede ser vacío
    /// </summary>
    public string unit_measure { get; set; } = string.Empty;
    public double total_amount { get; set; }
}
