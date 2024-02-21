using Microsoft.AspNetCore.SignalR;

namespace ASPNetCoreMPSignalR;

public interface IChatHub:IHubContext
{
    Task GetMessage(string message);
    string GetContextId(string clientId);
}
