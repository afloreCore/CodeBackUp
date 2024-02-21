using System.ComponentModel;
using System.Text.Json.Serialization;

namespace WrapperMercadoPagoAPI.Model;
internal class BasicListResponse: IAsyncDisposable
{

    [JsonPropertyName("error")]
    public string error { get; set; } = string.Empty;


    [JsonPropertyName("message")]
    public string messsage { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public int status { get; set; }

    [JsonPropertyName("causes")]
    public string[] causes { get; set; } = null!;

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
