using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
//using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using System.Threading;
using WrapperMercadoPagoAPI.Model;

namespace WrapperMercadoPagoAPI.Service;
public class ConfigurationService:IDisposable
{
    public static string baseUri => "https://api.mercadopago.com";
    public readonly string user_Id = ParameterService.UserId; //"1506231285";
    public readonly long sponsor_id = 136614767;
    //private readonly string token = "TEST-8182650202833324-082815-164cbc5ae5c89f1f86bd1fa2415b0c51-136614767";
    private readonly string token = ParameterService.Token; //"APP_USR-7467881949049174-101014-8992b07cffadbd3d966b4acb95c328f3-1506231285";
    private readonly HttpClient oClient;
    private JsonSerializerSettings oJsonSettings;
    private readonly string methodFullName;
    private const string div = "|";
    private bool _disposedValue;
    public ConfigurationService()
    {
        oClient = HttpClient();
        oJsonSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
        methodFullName = System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType!.FullName!.ToString() ?? string.Empty;    
    }
    public string FullNameMetod => methodFullName;
    public HttpClient HttpClient()
    {
        HttpClient cliente = new() { BaseAddress = new(new(baseUri)) };
        cliente.DefaultRequestHeaders.Add("Accept", "application/json");
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return cliente;
    }

    public async Task<T2?> PostAsync<T1, T2>(T1? obj, string uri, JsonSerializerSettings? jsonSettings = null)
    {
        try
        {
            var serialize = JsonConvert.SerializeObject(obj, (jsonSettings ?? jsonSettings));
            var content = new StringContent(serialize, Encoding.UTF8);
            using HttpResponseMessage response = await oClient.PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                if (res != null)
                {
                    var value = JsonConvert.DeserializeObject<T2>(res);
                    if (value != null)
                        return (T2)Convert.ChangeType(value, typeof(T2));
                }
            }
            else
                SetMessageFromRsponse(response.Content.ReadAsStringAsync().Result);
        }
        catch(Exception ex)
        {
            ErrorManager.SetErrorMesage(ex.Message, FullNameMetod);
            return default; 
        }

        return default;
    }
    public async Task<T?> GetAsync<T>(string uri)
    {
        try
        {
            using HttpResponseMessage response = await oClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                if (res != null)
                {
                    var value = JsonConvert.DeserializeObject<T>(res);
                    if (value != null)
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                }
            }
        }
        catch(Exception ex)
        {
            ErrorManager.SetErrorMesage(ex.Message, methodFullName);
            return default; 
        }
        return default;
    }
    public async Task<T?> GetAsync<T>(string uri, CancellationToken token)
    {
        string res = null!;

        while (!token.IsCancellationRequested && res == null)
        {
            try
            {
                using HttpResponseMessage response = await oClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    res = await response.Content.ReadAsStringAsync();
                }
            }
            catch 
            {
               return default; 
            }
        }
        if (res != null)
        {
            try
            {
                var value = JsonConvert.DeserializeObject<T>(res);
                if (value != null)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch (Exception ex)
            {
                ErrorManager.SetErrorMesage(ex.Message, methodFullName);
                return default;
            }
        }
        return default;
    }
    public async Task<bool> PutAsync<T>(T obj, string uri, JsonSerializerSettings? jsonSettings = null)
    {
        try
        {
            var serialize = JsonConvert.SerializeObject(obj, (jsonSettings ?? jsonSettings));
            var content = new StringContent(serialize, Encoding.UTF8);
            using HttpResponseMessage response = await oClient.PutAsync(uri, content);
            bool  request = response.IsSuccessStatusCode;
            if (!request) 
            {
                SetMessageFromRsponse(response.Content.ReadAsStringAsync().Result);
            }
            return request;
        }
        catch
        { return false; }
    }
    public async Task<T?>PutAsync<T, U>(U objSerialize,string uri, bool jsonSettings = false)
    {
        try
        {
            var serialize = JsonConvert.SerializeObject(objSerialize, (jsonSettings? oJsonSettings : null));
            var content = new StringContent(serialize, Encoding.UTF8);
            using HttpResponseMessage response = await oClient.PutAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                if (res != null)
                {
                    var sucs = JsonConvert.DeserializeObject<T>(res);
                    return sucs;
                }
            }
            else
            {
                SetMessageFromRsponse(response.Content.ReadAsStringAsync().Result);
            }
        }
        catch(Exception ex) 
        {
            ErrorManager.SetErrorMesage(ex.Message, methodFullName);
            return default; 
        }
        return default;
    }

    public async Task<bool> DeleteAsync(string uri)
    {
        using HttpResponseMessage response = await oClient.DeleteAsync(uri);
        return response.IsSuccessStatusCode;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (oJsonSettings != null)
                    oJsonSettings = null!;
                oClient.Dispose();
            }
            _disposedValue = true;
        }
    }

    public TResult? InvokeAsyncFuncSynchronously<TResult>(Task<TResult?> func, CancellationToken token)
    {
        TResult? result = default(TResult);
        var autoResetEvent = new AutoResetEvent(false);

        Task.Run(async () =>
        {
            if (token.IsCancellationRequested)
                return;
            try
            {
                result = await func;
            }
            catch (Exception exc)
            {
                BasicListResponse basicListResponse = new BasicListResponse()
                {
                     messsage = exc.Message,
                     status = 400
                };                      
                SetMessageFromRsponse(basicListResponse.ToString());

            }
            finally
            {
                autoResetEvent.Set();
            }
        });
        autoResetEvent.WaitOne();

        return result;
    }
    private void SetMessageFromRsponse(string? response)
    {
        if (response == null)
            return;
        BasicListResponse? err = JsonConvert.DeserializeObject<BasicListResponse>(response);
        if (err != null)
        {
            ErrorManager.SetErrorMesage(String.Concat(err.error, div, err.messsage, div, err.status, string.Join(string.Empty, err.causes)), methodFullName);
        }
    }
}
