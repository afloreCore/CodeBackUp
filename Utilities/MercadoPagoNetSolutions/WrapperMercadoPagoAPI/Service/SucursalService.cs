using Newtonsoft.Json;
using System.Text;
using WrapperMercadoPagoAPI.Model;

namespace WrapperMercadoPagoAPI.Service;
public class SucursalService : IDisposable
{
    private ConfigurationService? _configurationService;
    private HttpClient? _httpClient = null!;
    private readonly string _user_id;
    private bool _disposedValue;
    public SucursalService()
    {
        _configurationService = new ConfigurationService();
        _user_id = _configurationService.user_Id;
        _httpClient = _configurationService.HttpClient();
    }
    public async Task<Sucursales?> AddSucurs(Sucursal sucursal)
    {
        try
        {
            var serialize = JsonConvert.SerializeObject(sucursal);
            var content = new StringContent(serialize, Encoding.UTF8);
            using HttpResponseMessage response = await _httpClient!.PostAsync($"/users/{_user_id}/stores", content);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                if (res != null)
                {
                    var sucs = JsonConvert.DeserializeObject<Sucursales>(res);
                    return sucs;
                }
            }
            else
            {
                var ErrMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
            }
        }
        catch
        {
            return null;
        }
        return null;
    }
    public async Task<SucursalRequest?> UpdateSucurs(long id, Sucursal sucursal, bool jsonSettings = false)
    {
        var uri = $"users/{_user_id}/stores/{id}";
        return await _configurationService!.PutAsync<SucursalRequest, Sucursal>(sucursal, uri, jsonSettings);
    }

    public async Task<SucursalRequest?> GetSucursal(string name)
    {
        if (name == null)
            return null;
        try
        {
            var uri = $"users/{_configurationService!.user_Id}/stores/search?name={name}";
            var listSuc = await _configurationService.GetAsync<Sucursales>(uri);
            return listSuc!.results!.FirstOrDefault(x => x.name == name);
        }
        catch (Exception ex)
        {
            ErrorManager.SetErrorMesage(ex.Message, _configurationService!.FullNameMetod);
            return null;
        }

    }

    public async Task<Sucursal?> GetSucursal(long id)
    {
        if (id > 0)
        {
            var uri = $"stores/{id}";
            return await _configurationService!.GetAsync<Sucursal>(uri);
        }
        return default;
    }
    public async Task<Sucursales?> GetSucursal(long id = 0, string extID = "")
    {
        var uri = id > 0 ? $"stores/{id}" : $"users/{_configurationService!.user_Id}/stores/search?" + (extID.Length > 0 ? $"external_id={extID}" : string.Empty);
        return await _configurationService!.GetAsync<Sucursales>(uri);
    }
    public async Task<bool?> DelSucursal(string id)
    {
        var uri = $"users/{_user_id}/stores/{id}";
        return await _configurationService!.DeleteAsync(uri);
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
                _configurationService = null;
                _httpClient = null;
            }

            _disposedValue = true;
        }
    }
}
