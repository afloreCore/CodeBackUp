namespace ASPNetCoreMPSignalR;

public class ConnectionMapping
{
    private static readonly Dictionary<string, string> _connections = new();
    public int Count
    {
        get
        {
            return _connections.Count;
        }
    }
    public void Add(string key, string connectionId)
    {
        lock (_connections)
        {
            if (!_connections.TryGetValue(key, out _))
            {
                _connections.Add(key, connectionId);
            }
        }
    }
    public string GetConnections(string key)
    {
        if (key == null) { return string.Empty; }
        if (_connections.TryGetValue(key, out var connection))
        {
            return connection;
        }
        return string.Empty;
    }
    public void RemoveFromValue(string? value) 
    { 
        lock (_connections) 
        {
            var reg = _connections.FirstOrDefault(x => x.Value == value);
            var _key = reg.Key?? string.Empty;
            if (_connections.TryGetValue(_key, out _))
            {
                _connections.Remove(_key);
            }
        }
    }
    public void RemoveAll()
    {

            lock (_connections)
            {
            foreach (var key in _connections.Keys)
                _connections.Remove(key);
            }
    }
}
