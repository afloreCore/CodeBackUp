using Microsoft.AspNetCore.SignalR;

namespace ASPNetCoreMPSignalR;

public class ChatHub : Hub<IChatHub>
{

    protected IHubContext<ChatHub> _context;
    protected readonly ConnectionMapping connections;    
    public ChatHub(IHubContext<ChatHub> context)
    {
        _context = context;
        connections = new();
    }
    public async Task<bool> SendMessage(string? userId, string? topic, string? operationID)
    {
        if(userId == null) { return false; }
        string connectionId = connections.GetConnections(userId); 
        if (connectionId == null) { return false; } 
        return await _context.Clients.Client(connectionId).InvokeAsync<bool>("SignalRMessage", topic, operationID, CancellationToken.None);
    }
    public string GetContextId(string clientId)
    {
        var connectionId = Context.ConnectionId;
        connectionId = connections.GetConnections(clientId);
        return connectionId;
    }
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var httpContext = Context.GetHttpContext();
        var key = httpContext!.Request.Query["clientId"].ToString();
        if (key == null) { return; }
        if (connections.GetConnections(key) != null)
        {
            connections.Add(key , connectionId);
        }
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        connections.RemoveFromValue(connectionId);
        return base.OnDisconnectedAsync(exception);
    }

}
