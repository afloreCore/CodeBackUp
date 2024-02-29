using Newtonsoft.Json;
using WrapperMercadoPagoAPI.Model;

namespace WrapperMercadoPagoAPI.Service;
internal class OrderService : IDisposable
{
    private bool _disposedValue;
    private readonly ConfigurationService _configurationService;

    public OrderService()
    {
        _configurationService = new ConfigurationService();
    }

    /// <summary>
    /// Genera una orden de cobro
    /// </summary>
    /// <param name="order">Clase Orden con los valores de la compra</param>
    /// <param name="extStoreID">external_id retornado por la api luego de la creación de una sucursal/Store</param>
    /// <param name="extPosID">external_id retornado luego de la creación de una caja/Branch. Relacionado a una sucursal</param>
    /// <returns></returns>
    public async Task<bool> CreateOrder(Order order, string extStoreID, string extPosID)
    {
        if (order == null)
        {
            return false;
        }
        //if(_configurationService.sponsor_id > 0)
        //{
        //    order.sponsor = new() { id = _configurationService.sponsor_id };
        //}
        JsonSerializerSettings serializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
        var uri = $"instore/qr/seller/collectors/{_configurationService.user_Id}/stores/{extStoreID}/pos/{extPosID}/orders";
        return await _configurationService.PutAsync<Order>(order, uri, serializerSettings);
    }
    public async Task<Order?> GetOrder(string extPosID)
    {
        var uri = $"https://api.mercadopago.com/instore/qr/seller/collectors/{_configurationService.user_Id}/pos/{extPosID}/orders";
        return await _configurationService.GetAsync<Order>(uri);
    }
    public async Task<Order?> GetOrderFromId(string orderID)
    {
        var uri = $"merchant_orders/{orderID}";
        return await _configurationService.GetAsync<Order>(uri);
    }
    public async Task<bool> CancelOrder(string extPosID)
    {
        var uri = $"https://api.mercadopago.com/instore/qr/seller/collectors/{_configurationService.user_Id}/pos/{extPosID}/orders";
        return await _configurationService.DeleteAsync(uri);
    }
    public void Dispose()
    {
        Dispose(true);
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

