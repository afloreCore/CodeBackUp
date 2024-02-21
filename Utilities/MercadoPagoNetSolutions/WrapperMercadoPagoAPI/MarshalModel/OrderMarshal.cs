using System.Runtime.InteropServices;
using WrapperMercadoPagoAPI.Model;
using WrapperMercadoPagoAPI.Enum;

namespace WrapperMercadoPagoAPI.MarshalModel;
[ComVisible(true), Guid("F7768CE9-9D6C-47FF-817F-1D3241AD736F"), ClassInterface(ClassInterfaceType.AutoDispatch), ProgId("OrderMarshal")]
public class OrderMarshal
{
    public string external_reference = "";
    public PaymentStatus status { get; set; }

    public string title = "";

    public double total_amount;

    public string description = "";

    public string expiration_date = "";

    [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] 
    public object[] items = null!;

    public double cash_out;

    public string sponsor = "";
}
