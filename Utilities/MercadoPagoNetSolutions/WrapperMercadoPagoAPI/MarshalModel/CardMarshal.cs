using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.MarshalModel;
[ComVisible(true), Guid("25825CE5-B627-4EDF-86B2-8F700366D1EB"), ClassInterface(ClassInterfaceType.AutoDispatch), ProgId("WrapperMercadoPagoAPI.CardMarshal")]
public class CardMarshal
{
    public string bin { get; set; } = string.Empty;
    public DateTime date_created { get; set; }
    public DateTime date_last_updated { get; set; }
    public int expiration_month { get; set; }
    public int expiration_year { get; set; }
    public string first_six_digits { get; set; } = string.Empty;
    public string id { get; set; } = string.Empty;
    public string last_four_digits { get; set; } = string.Empty;
}
