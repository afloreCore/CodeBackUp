using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.MarshalModel;
[ComVisible(true), Guid("4CF584A8-0FE2-4D7E-98E0-BDCA321D20A7"), ClassInterface(ClassInterfaceType.AutoDispatch), ProgId("SucursalMarshal")]
public class SucursalMarshal
{
    public long id { get; set; }
    public string name { get; set; } = string.Empty;
    public string external_id { get; set; } = string.Empty;
}

