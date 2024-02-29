using WrapperMercadoPagoAPI.General;
using WrapperMercadoPagoAPI.Model;

namespace WrapperMercadoPagoAPI.Service;
public class PaymentService : IDisposable
{
    private bool _disposedValue;
    private readonly ConfigurationService _configurationService;

    public PaymentService()
    {
        _configurationService = new ConfigurationService();
    }

    public async Task<PaymentRequest?> GetPaymentFromId(string id)
    {
        var uri = $"v1/payments/{id}";
        return await _configurationService.GetAsync<PaymentRequest>(uri);
    }
    public async Task<Payments?> GetPayments(DateTime? beginDate, DateTime? endDate, string storeID = "", string posID = "", string externalReference = "")
    {
        string dateParameters = string.Empty;
        string statusParameters = string.Empty;
        if (beginDate.HasValue)
        {
            string begin = Utilities.GetStrDateISO8601(beginDate ?? DateTime.Now.AddDays(-30));
            string end = Utilities.GetStrDateISO8601(endDate ?? DateTime.Now);
            dateParameters = $"sort=date_created&criteria=desc&range=date_created&begin_date={begin}&end_date={end}";
        }
        else
            statusParameters = "&status=approved";

        //https://api.mercadopago.com/v1/payments/search?sort=date_created&criteria=desc&external_reference=FB0001-0003&range=date_created&begin_date=NOW-30DAYS&end_date=NOW&store_id=57663869&pos_id=87219375
        var uri = $"v1/payments/search?" + dateParameters + (storeID.Length == 0 ? string.Empty : $"&store_id={storeID}") + (posID.Length == 0 ? string.Empty : $"&pos_id={posID}")
        + (externalReference.Length == 0 ? string.Empty : $"&external_reference={externalReference}") + (statusParameters.Length == 0 ? string.Empty : statusParameters);

        return await _configurationService.GetAsync<Payments>(uri);
    }

    public async Task<Payments?> GetAndWaitPayment(string storeID, string posID, string externalReference, CancellationToken token, PaymentStatus paystatus)
    {
        //busco solamente el aprobado, sino devolverá null
        //sort=date_created&criteria=desc : Ordenado de mayor fecha a menor
        var uri = $"v1/payments/search?sort=date_created&criteria=desc&store_id={storeID}&pos_id={posID}&external_reference={externalReference}"
            + (paystatus != PaymentStatus.None ? $"&status={System.Enum.GetName(typeof(PaymentStatus), paystatus)}" : string.Empty);
        return await _configurationService.GetAsync<Payments>(uri, token);
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
