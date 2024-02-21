using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.MarshalModel;
[ComVisible(true), Guid("A911C475-F5E5-401E-9542-E6D3465863EE"), ProgId("PaymentsMarshal")]
public class PaymentsMarshal
{

    [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
    public object[] results = null!;
}
