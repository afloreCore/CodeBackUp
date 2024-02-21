using InterfaceComNetCore;
using HubConnectionWrapper;
using System.Runtime.InteropServices;
using WrapperMercadoPagoAPI.Interface;

namespace WrapperMercadoPagoAPI.General;
[ComVisible(true), ClassInterface(ClassInterfaceType.None), Guid("126717A3-6BFA-40F5-A3E8-E81643E8F8A7"), ProgId("WrapperMercadoPagoAPI.ClassInterop")]
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
        if(userId == null) { return string.Empty; }
        try
        {
            return wrapperCallback.GetConnectionId(userId)?? string.Empty;
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
