using System.Collections;
using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.MarshalModel;
[ComVisible(true), Guid("F392526B-6A2D-4183-8DF1-6BBFF3108718"), ClassInterface(ClassInterfaceType.AutoDispatch), ProgId("WrapperMercadoPagoAPI.SucursalRequestMarshal")]
public class SucursalRequestMarshal
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = null!;
    public string date_creation { get; set; } = string.Empty;
    public string external_id { get; set; } = null!;

}

