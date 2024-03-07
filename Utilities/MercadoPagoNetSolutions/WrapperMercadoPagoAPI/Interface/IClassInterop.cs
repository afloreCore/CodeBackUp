using InterfaceComNetCore;
using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.Interface;
[ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("676550F0-3836-4811-A41E-7B38092B1AB9")]
public interface IClassInterop
{
    public bool Initialize(string HttpsHost, string fileSeparation, string listSeparation, string valueSeparation, string queryParameters = "");
    public string GetConnectionId(string userId);
    public ICallbackInterop ReturnValue { get; set; }
}
