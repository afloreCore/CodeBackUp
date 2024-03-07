using HubConnectionWrapper;
using InterfaceComNetCore;
using System.Runtime.InteropServices;
using System.Text;
using WrapperMercadoPagoAPI.Interface;
using WrapperMercadoPagoAPI.Service;

namespace WrapperMercadoPagoAPI.General;
[ComVisible(true), ClassInterface(ClassInterfaceType.None), Guid("175A6CE3-66DB-427A-8498-D665120D4FAA"), ProgId("WrapperMercadoPagoAPI.ClassInterop")]
public class ClassInterop : IClassInterop
{
    private WrapperCallback wrapperCallback = null!;
    private static string fileSep = string.Empty;
    private static string listSep = string.Empty;
    private static string valueSep = string.Empty;
     
    public ClassInterop(){ } 
    public bool Initialize(string HttpsHost, string fileSeparation, string listSeparation, string valueSeparation, string queryParameters = "")
    { 
        try
        {
            (fileSep, listSep, valueSep) = (fileSeparation, listSeparation, valueSeparation);
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
        return long.TryParse(id, out _) ? Utilities.WriteTextFile(ParameterService.FullPathTmpFile, string.Concat("topic=", topic, "#id=", id)) : false;
    }
}