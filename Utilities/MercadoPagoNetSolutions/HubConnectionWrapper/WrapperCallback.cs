using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;


namespace HubConnectionWrapper
{
    public class WrapperCallback
    {
        protected HubConnection connection = null!;
        public delegate bool CallbackReturn(string operation, string operationId);
        public event CallbackReturn CallbackEvent = null!;
        
        //public async Task<bool>SendMessage(string? topic, string? operationID)
        public bool StartConnection(string httpsHost, string queryParameters = "")
        {
            //var result = await connection.InvokeAsync<bool>("SendMessage", "test","ok", CancellationToken.None);
            //return result;
            bool result = Task.Run(() => Initialize(httpsHost, queryParameters)).GetAwaiter().GetResult();
            return result;
        }
        public string? GetConnectionId(string userId)
        {
            return connection?.InvokeAsync<string>("GetContextId", userId, CancellationToken.None).GetAwaiter().GetResult();
        }
        private async Task<bool> Initialize(string httpsHost, string queryParameters = "")
        {
            var parameters = (queryParameters.Length > 0 ? $"?{queryParameters}" : string.Empty);
            connection = new HubConnectionBuilder().WithUrl($"{httpsHost}/chatHub{parameters}")
            .WithAutomaticReconnect()
            .Build();
            connection.Closed += async (error) =>
            {
                Debug.Print("await connection.StartAsync()");
                await connection.StartAsync();
                Debug.Print("Connected");

            };
            //SignalRMessage es el método con el que se comunicará el server
            //El tercer parámetro <bool> es el tip de retorno (si se quiere devolver la llamada)
            connection.On<string, string, bool>("SignalRMessage", (topic, id) =>
            {
                return ReturnCallback(topic, id);
            });
            try
            {
                Debug.Print("await connection.StartAsync()");
                await connection.StartAsync();
                Debug.Print("Connected");
                return true;
            }
            catch(HubException ex)
            {
                return false;
            }

        }
        private bool ReturnCallback(string topic, string id)
        {
            return CallbackEvent.Invoke(topic, id);
        }
    }
}
