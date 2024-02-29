using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.MarshalModel;
[ComVisible(true), Guid("83FE1581-A601-4EF4-8897-7B5D9BAE9C21"), ClassInterface(ClassInterfaceType.AutoDispatch), ProgId("BranchRequestMarshal")]
public class BranchRequestMarshal
{
    public int id { get; set; }
    public string linkImageQr { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public DateTime date_created { get; set; }
    public DateTime date_last_updated { get; set; }
    public string uuid { get; set; } = string.Empty;
    public long user_id { get; set; }
    public string name { get; set; } = string.Empty;
    public bool fixed_amount { get; set; }
    public BranchCategories category { get; set; }
    public string store_id { get; set; } = string.Empty;
    public string external_store_id { get; set; } = string.Empty;
    public string external_id { get; set; } = string.Empty;
    public string site { get; set; } = string.Empty;
    public string qr_code { get; set; } = string.Empty;

}
