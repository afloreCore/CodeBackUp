using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using WrapperMercadoPagoAPI.General;
using WrapperMercadoPagoAPI.Model;
using static System.Net.WebRequestMethods;

namespace WrapperMercadoPagoAPI.Service;
public class BranchService:IDisposable
{
    private readonly ConfigurationService _configurationService;
    private bool _disposedValue;
    public BranchService()
    {
        _configurationService = new ConfigurationService();
    }

    public async Task<BranchRequest?> AddBranch(Branch br, JsonSerializerSettings? jsonSettings = null)
    {

        var uri = @"https://api.mercadopago.com/pos";
        return await _configurationService.PostAsync<Branch, BranchRequest>(br, uri, jsonSettings);
    }
    public async Task<Branchs?>GetBranch(Branch br, bool nullValue = true)
    {
        if(br == null)
            return null;

        StringBuilder _uri = new StringBuilder().Append("pos");
        var strQuery = Utilities.MakeUriParameters(br, nullValue); 
        if (strQuery.Length > 0)
            _uri.Append('?').Append(strQuery);
        
        return await _configurationService.GetAsync<Branchs>(_uri.ToString());
    }
    public async Task<BranchRequest?> UpdateBranch(long id, Branch br)
    {
        var uri = $"pos/{id}";
        return await _configurationService.PutAsync<BranchRequest, Branch>(br, uri, true);
    }
    public async Task<bool> DeleteBranch(long id)
    {

        var uri = $"pos/{id}";
        Branch br = new() { id = id };
        return await _configurationService.DeleteAsync(uri);
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
                _configurationService.Dispose();
            }

            _disposedValue = true;
        }
    }
}
