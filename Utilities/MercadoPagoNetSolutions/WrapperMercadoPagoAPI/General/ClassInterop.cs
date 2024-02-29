using HubConnectionWrapper;
using InterfaceComNetCore;
using System.Runtime.InteropServices;
using WrapperMercadoPagoAPI.Interface;

namespace WrapperMercadoPagoAPI.General;
[ComVisible(true), ClassInterface(ClassInterfaceType.None), Guid("175A6CE3-66DB-427A-8498-D665120D4FAA"), ProgId("WrapperMercadoPagoAPI.ClassInterop")]
public class ClassInterop : IClassInterop
{
    private WrapperCallback wrapperCallback = null!;
    public ClassInterop()
    {
    }
    public bool Initialize(string HttpsHost, string queryParameters = "")
    {
        try
        {
            wrapperCallback = new WrapperCallback();
            wrapperCallback.CallbackEvent += CallbackEvent;
            return wrapperCallback.StartConnection(HttpsHost, queryParameters);
        }
        catch
        {
            return false;
        }
    }
    public string GetConnectionId(string userId)
    {
        if (userId == null) { return string.Empty; }
        try
        {
            return wrapperCallback.GetConnectionId(userId) ?? string.Empty;
        }
        catch { return string.Empty; }
    }
    public ICallbackInterop ReturnValue { get; set; } = null!;
    private bool CallbackEvent(string topic, string id)
    {
        try
        {
            var result = ReturnValue?.ReturnValue(topic, id);
            return result ?? false;
        }
        catch { return false; }
    }
}
