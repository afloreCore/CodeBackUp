using InterfaceComNetCore;
using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.Interface;
[ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("AC7E5DD9-1D5C-49F2-999A-51FBE67014E5")]
public interface IClassInterop
{
    public bool Initialize(string HttpsHost, string queryParameters = "");
    public string GetConnectionId(string userId);
    public ICallbackInterop ReturnValue { get; set; }
}
