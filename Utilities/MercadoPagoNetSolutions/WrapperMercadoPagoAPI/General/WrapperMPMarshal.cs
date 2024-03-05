using InterfaceComNetCore;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using WrapperMercadoPagoAPI.Interface;
using WrapperMercadoPagoAPI.MarshalModel;
using WrapperMercadoPagoAPI.Model;
using WrapperMercadoPagoAPI.Service;

namespace WrapperMercadoPagoAPI.General;
[ComVisible(true), Guid("21500559-8C0C-40CF-A009-AD47B9C0D31D"), ProgId("WrapperMercadoPagoAPI.WrapperMPMarshal"), ComDefaultInterface(typeof(IWrapperMarshal))]
public class WrapperMPMarshal : IWrapperMarshal
{

    public WrapperMPMarshal() { }
    public void InitializeMpMarshal(string userId, string token, string fullPathTmpFile)
    {
        ParameterService.UserId = userId;
        ParameterService.Token = token;
        ParameterService.FullPathTmpFile = fullPathTmpFile;
    }
    public ICallbackInterop ReturnValue { get; set; } = null!;
    #region Operaciones con Sucursales/Stores
    SucursalRequestMarshal? IWrapperMarshal.GetSucursalFromName(string name)
    {
        using SucursalService ss = new();
        var result = Task.Run(() => ss.GetSucursal(name).Result);
        SucursalRequest? r = result.GetAwaiter().GetResult();
        if (r != null)
        {
            return new()
            {
                name = r.name,
                external_id = r.external_id,
                id = r.id,
                date_creation = r.date_creation
            };
        }
        return default;
    }
    SucursalRequestMarshal? IWrapperMarshal.GetSucursalFromId(long sucId)
    {
        using SucursalService ss = new();
        var result = Task<Sucursal?>.Run(() => ss.GetSucursal(id: sucId));
        var r = result.GetAwaiter().GetResult();
        if (r != null)
        {
            return new()
            {
                name = r.name,
                external_id = r.external_id,
                id = r.id.ToString(),
                date_creation = r.date_creation
            };
        }
        return default;

    }
    public SucursalesMarshal? GetSucursales(long id, string external_id)
    {
        using SucursalService ss = new();
        var result = Task.Run(() => ss.GetSucursal(id, external_id).Result);
        var r = result.GetAwaiter().GetResult();
        if (r != null)
        {
            List<SucursalRequestMarshal> list = new();
            foreach (var item in r.results)
            {
                var s = new SucursalRequestMarshal()
                {
                    date_creation = item.date_creation,
                    external_id = item.external_id,
                    id = item.id,
                    name = item.name
                };
                list.Add(s);
            }
            return new() { results = list };
        }
        return default;

    }
    public SucursalRequestMarshal? UpdateSucursal(long id, string name, string external_id)
    {
        if (id <= 0 || name.Length == 0 || external_id.Length == 0)
        {
            ErrorManager.SetErrorMesage("id, name or external_id invalid", this.ToString());
            return default;
        }
        try
        {
            Sucursal suc = new()
            {
                name = name,
                external_id = external_id
            };
            using SucursalService ss = new();
            var result = Task<SucursalRequest?>.FromResult(ss.UpdateSucurs(id, suc, true)).Result;
            var r = result.GetAwaiter().GetResult();
            if (r != null)
            {
                return new()
                {
                    name = r!.name ?? string.Empty,
                    date_creation = r!.date_creation ?? string.Empty,
                    external_id = r!.external_id ?? string.Empty,
                    id = r!.id ?? string.Empty
                };
            }
        }
        catch (Exception ex)
        {
            ErrorManager.SetErrorMesage(ex.Message, this.ToString());
        }
        return default;
    }
    #endregion Sucursales/Stores
    #region Operaciones con Brunchs/cajas
    BranchRequestMarshal? IWrapperMarshal.CreateBranch(long storeId, string externalId, string nameInput, bool fixedAmount, BranchCategories inCategory)
    {
        try
        {
            Branch br = new()
            {
                name = nameInput,
                store_id = storeId,
                external_id = externalId,
                category = inCategory,
                fixed_amount = fixedAmount
            };

            using BranchService ss = new();
            JsonSerializerSettings settings = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var result = Task.Run(() => ss.AddBranch(br, settings).Result);
            var r = result.GetAwaiter().GetResult();
            if (r != null)
            {
                return new()
                {
                    external_id = r!.external_id ?? string.Empty,
                    category = r!.category,
                    fixed_amount = r!.fixed_amount,
                    external_store_id = r!.external_store_id ?? string.Empty,
                    date_created = r!.date_created,
                    status = r!.status,
                    id = r!.id,
                    name = r!.name ?? string.Empty,
                    linkImageQr = r!.qr!.image ?? string.Empty,
                    store_id = r!.store_id ?? string.Empty,
                    site = r!.site ?? string.Empty,
                    user_id = r!.user_id
                };
            }
        }
        catch { return default; }
        return default;
    }

    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
    public object?[] GetBranchs(long storeId, string externalId, string extStoreid, long posid)
    {

        using BranchService ss = new();
        JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
        Branch br = new()
        {
            external_id = externalId,
            external_store_id = extStoreid,
            store_id = storeId,
            id = (posid > 0 ? posid : 0)
        };
        var result = Task.Run(() => ss.GetBranch(br).Result);
        var r = result.GetAwaiter().GetResult();
        if (r != null)
        {
            List<BranchRequestMarshal> list = new();
            foreach (var item in r.results)
            {
                var _br = new BranchRequestMarshal()
                {
                    id = item.id,
                    category = item.category,
                    external_id = item.external_id,
                    date_created = item.date_created,
                    external_store_id = item.external_store_id,
                    date_last_updated = item.date_last_updated,
                    fixed_amount = item.fixed_amount,
                    linkImageQr = (item.qr == null ? string.Empty : item.qr.image.ToString()),
                    name = item.name,
                    site = item.site
                };
                list.Add(_br);
            }
            return list.ToArray<object>();
        }

        return null!;
    }
    public bool DeleteBrunch(long id)
    {
        using BranchService ss = new();
        JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
        var result = Task.Run(() => ss.DeleteBranch(id).Result);
        return result.GetAwaiter().GetResult();
    }
    BranchRequestMarshal? IWrapperMarshal.UpdateBrunch(long id, string name, bool fixed_amount, BranchCategories category)
    {
        if (id == 0)
            return default;
        try
        {
            using BranchService ss = new();
            Branch br = new()
            {
                name = name,
                fixed_amount = fixed_amount,
                category = category
            };
            var result = Task.Run(() => ss.UpdateBranch(id, br).Result);
            var r = result.GetAwaiter().GetResult();
            if (r != null)
            {
                return new()
                {
                    name = r.name ?? string.Empty,
                    fixed_amount = r.fixed_amount,
                    category = r.category,
                    date_created = r.date_created,
                    date_last_updated = r.date_last_updated,
                    external_id = r.external_id,
                    external_store_id = r.external_store_id,
                    id = r.id,
                    linkImageQr = r!.qr!.image ?? string.Empty,
                    status = r.status ?? string.Empty,
                    store_id = r.store_id ?? string.Empty,
                    user_id = r.user_id
                };

            }
        }
        catch (Exception ex)
        {
            ErrorManager.SetErrorMesage(ex.Message, this.ToString());
            return default;
        }
        return default;
    }

    #endregion Branchs

    #region Operaciones con Ordenes/Orders
    //int AddItemToOrder(double unitPrice, double quantity, double totalAmout, string skuNumber = "", string title = "", string category = "", string description = "", string unitMesure = "");
    public int AddItemToOrder(double unitPrice, double quantity, double totalAmout, string skuNumber, string title, string category, string description, string unitMesure)
    {
        if (unitPrice <= 0 || totalAmout <= 0)
            return 0;
        try
        {
            ItemOrder itemOrder = new()
            {
                unit_price = unitPrice,
                quantity = quantity,
                total_amount = totalAmout,
                sku_number = skuNumber,
                title = title,
                category = category,
                description = description,
                unit_measure = unitMesure
            };
            listItemOrder().Add(itemOrder);
            return listItemOrder().Count;
        }
        catch
        {
            return 0;
        }
    }
    bool IWrapperMarshal.CreateSingleItemOrder(string externalStoreID, string externalPosID, double totalAmount, string itemDescription, string itemUnit, DateTime expidateDateTime,
        string notificationUrl, double cashOut, string externalReference, string title, string desription)
    {
        try
        {
            using OrderService os = new();
            ItemOrder itemOrder = new()
            {
                sku_number = itemDescription,
                title = itemDescription,
                description = itemDescription,
                total_amount = totalAmount - cashOut,
                unit_measure = itemUnit,
                unit_price = totalAmount - cashOut,
                quantity = 1
            };
            using Order order = new()
            {
                total_amount = totalAmount,
                cash_out = new() { amount = cashOut },
                external_reference = externalReference,
                title = title,
                description = desription,
                expiration_date = Utilities.GetStrDateISO8601(expidateDateTime),
                notification_url = notificationUrl,
                items = new() { itemOrder }
            };
            var result = Task.Run(() => os.CreateOrder(order, externalStoreID, externalPosID).Result);
            return result.GetAwaiter().GetResult();
        }
        catch
        {
            return false;
        }

    }
    public bool CreateListItemOrder(string externalStoreID, string externalPosID, double totalAmount, double cashOut, string externalReference, string title, string desription, double timeOutSecond)
    {
        if (_listitemOrder != null)
        {
            try
            {
                using OrderService os = new();
                var format = "yyyy-MM-ddTHH:mm:ss.fff-HH:mm";
                using Order order = new()
                {
                    total_amount = totalAmount,
                    cash_out = new() { amount = cashOut },
                    external_reference = externalReference,
                    title = title,
                    description = desription,
                    expiration_date = DateTime.Now.AddSeconds(timeOutSecond).ToString(format),
                    items = _listitemOrder
                };
                var result = Task.Run(() => os.CreateOrder(order, externalStoreID, externalPosID).Result);
                return result.GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }
        return false;
    }

    Order? IWrapperMarshal.GetOrder(string externalPosID)
    {
        using OrderService os = new();
        var result = Task.Run(() => os.GetOrder(externalPosID).Result);
        return result.GetAwaiter().GetResult();
    }

    OrderMarshal? IWrapperMarshal.GetOrderFromId(string orderID)
    {
        using OrderService os = new();
        var result = Task.Run(() => os.GetOrderFromId(orderID).Result);
        if (result == null)
            return null;
        Order? order = result.GetAwaiter().GetResult();
        OrderMarshal om = new OrderMarshal()
        {
            cash_out = (order!.cash_out != null ? order!.cash_out.amount : 0),
            description = order.description,
            expiration_date = order.expiration_date,
            external_reference = order.external_reference,
            items = order.items.Cast<object>().ToArray(),
            total_amount = order.total_amount,
            title = order.title,
            sponsor = (order!.sponsor != null ? order.sponsor.id.ToString() : "")
        };
        return om;
    }
    OrderMarshal? GetMarshalOrder(string externalPosID)
    {
        using OrderService os = new();
        var result = Task.Run(() => os.GetOrder(externalPosID).Result);
        if (result == null)
            return null;
        Order? order = result.Result;
        OrderMarshal om = new OrderMarshal()
        {
            cash_out = (order!.cash_out != null ? order!.cash_out.amount : 0),
            description = order.description,
            expiration_date = order.expiration_date,
            external_reference = order.external_reference,
            items = order.items.Cast<ItemOrderMarshal>().ToArray(),
            total_amount = order.total_amount,
            title = order.title,
            status = Utilities.CastEnum<PaymentStatus>(order!.status.ToString()),
            sponsor = (order!.sponsor != null ? order.sponsor.id.ToString() : "")
        };
        return om;
    }

    bool IWrapperMarshal.CancelOrder(string externalPosID)
    {
        using OrderService os = new();
        var result = Task.Run(() => os.CancelOrder(externalPosID).Result);
        return result.GetAwaiter().GetResult();
    }

    private List<ItemOrder> _listitemOrder = null!;

    private List<ItemOrder> listItemOrder()
    {
        _listitemOrder ??= new List<ItemOrder>();
        return _listitemOrder;
    }
    #endregion Fin Ordenes/Orders
    #region Payment/Pagos
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
    object[] IWrapperMarshal.GetListPayments(DateTime? beginDate, DateTime? endDate, string storeID, string posID, string externalReference)
    {
        List<PaymentRequestMarshal> payments = null!;
        using PaymentService os = new();
        var result = Task.Run(() => os.GetPayments(beginDate, endDate, storeID, posID, externalReference).Result);
        var r = result.GetAwaiter().GetResult();
        if (r != null)
        {
            payments = new List<PaymentRequestMarshal>();
            foreach (PaymentRequest? item in r.results)
            {
                payments.Add(CastPaymentToMarshal(item));
            }
            return payments.ToArray<object>();
        }
        return null!;
    }
    PaymentRequestMarshal? IWrapperMarshal.GetPaymentFromId(string paymentId)
    {
        using PaymentService os = new();
        var result = Task.Run(() => os.GetPaymentFromId(paymentId).Result).GetAwaiter().GetResult();
        return CastPaymentToMarshal(result);
    }
    PaymentRequestMarshal? IWrapperMarshal.GetAndWaitPayment(string storeID, string posID, string externalReference, long timeout, PaymentStatus paystatus)
    {
        PaymentRequest? pay = null!;
        PaymentStatus paymentStatus = PaymentStatus.None;
        //multiplico x 1000 xq CancelationToken recibe milisegundos
        var cancellationTokenSource = new CancellationTokenSource((int)timeout * 1000);
        CancellationToken token = cancellationTokenSource.Token;
        using ConfigurationService os = new();
        using PaymentService ps = new();
        while (!token.IsCancellationRequested && paymentStatus != PaymentStatus.approved)
        {
            try
            {
                var request = os.InvokeAsyncFuncSynchronously<Payments?>(ps.GetPayments(null, null, storeID, posID, externalReference), token);
                if (request != null)
                    if (request.results.Count > 0)
                    {
                        pay = request.results.FirstOrDefault();
                        paymentStatus = pay!.status;
                    }
            }
            catch
            {
                return default;
            }

        }
        cancellationTokenSource.Dispose();
        if (pay != null)
            return CastPaymentToMarshal(pay);
        return default;
    }
    PaymentRequestMarshal? IWrapperMarshal.GetPayment(string storeID, string posID, string externalReference)
    {
        using PaymentService os = new();
        var result = Task.Run(() => os.GetPayments(null, null, storeID, posID, externalReference).Result);
        var listPayment = result.GetAwaiter().GetResult();
        if (listPayment != null)
        {
            var r = listPayment.results.FirstOrDefault();
            if (r != null)
            {
                return CastPaymentToMarshal(r);
                //if (r.card != null)
                //{
                //    var _card = r.card;
                //    cardMarshal = new()
                //    {
                //        bin = _card.bin,
                //        date_created = _card.date_created ?? DateTime.MinValue,
                //        date_last_updated = _card.date_last_updated ?? DateTime.MinValue,
                //        expiration_month = _card.expiration_month,
                //        expiration_year = _card.expiration_year,
                //        first_six_digits = _card.first_six_digits ?? string.Empty,
                //        id = _card.id ?? string.Empty,
                //        last_four_digits = _card.last_four_digits ?? string.Empty
                //    };
                //}
                //return new()
                //{
                //    authorization_code = r.authorization_code ?? string.Empty,
                //    binary_mode = r.binary_mode,
                //    card = cardMarshal,
                //    date_created = r.date_created ?? DateTime.MinValue,
                //    date_last_updated = r.date_last_updated,
                //    collector_id = r.collector_id,
                //    coupon_amount = r.coupon_amount,
                //    currency_id = r.currency_id.ToString(),
                //    date_approved = r.date_approved ?? DateTime.MinValue,
                //    external_reference = r.external_reference ?? string.Empty,
                //    description = r.description ?? string.Empty,
                //    money_release_date = r.money_release_date ?? DateTime.MinValue,
                //    id = r.id,
                //    payment_method_id = r.payment_method_id,
                //    payment_type_id = r.payment_type_id.ToString(),
                //    status = r.status.ToString(),
                //    status_detail = r.status_detail.ToString(),
                //    transaction_amount = r.transaction_amount,
                //    total_paid_amount = r!.transaction_details!.total_paid_amount,
                //    transaction_amount_refunded = r.transaction_amount_refunded,
                //    net_received_amount = r!.transaction_details!.net_received_amount,
                //    installments = r!.installments
                //};
            }
        }
        return default;
    }
    /*********************************************************************************************************/
    /*                                          UTILITIES                                                    */
    /*********************************************************************************************************/
    string IWrapperMarshal.GetStrDateISO8601(DateTime date)
    {
        return Utilities.GetStrDateISO8601(date);
    }
    DateTime IWrapperMarshal.GetDateTimeFromISO8601(string date)
    {
        return Utilities.GetDateTimeFromISO8601(date);
    }

    /*********************************************************************************************************/

    private PaymentRequestMarshal CastPaymentToMarshal(PaymentRequest? r)
    {
        CardMarshal cardMarshal = null!;
        if (r != null)
        {
            if (r.card != null)
            {
                var _card = r.card;
                cardMarshal = new()
                {
                    bin = _card.bin,
                    date_created = _card.date_created ?? DateTime.MinValue,
                    date_last_updated = _card.date_last_updated ?? DateTime.MinValue,
                    expiration_month = _card.expiration_month,
                    expiration_year = _card.expiration_year,
                    first_six_digits = _card.first_six_digits ?? string.Empty,
                    id = _card.id ?? string.Empty,
                    last_four_digits = _card.last_four_digits ?? string.Empty
                };
            }
            return new()
            {
                authorization_code = r.authorization_code ?? string.Empty,
                binary_mode = r.binary_mode,
                card = cardMarshal,
                date_created = r.date_created ?? DateTime.MinValue,
                date_last_updated = r.date_last_updated,
                collector_id = r.collector_id,
                coupon_amount = r.coupon_amount,
                currency_id = r.currency_id.ToString(),
                date_approved = r.date_approved ?? DateTime.MinValue,
                external_reference = r.external_reference ?? string.Empty,
                description = r.description ?? string.Empty,
                money_release_date = r.money_release_date ?? DateTime.MinValue,
                id = r.id,
                payment_method_id = Utilities.CastEnum<PaymentMethodID>(r.payment_method_id),
                payment_type_id = Utilities.CastEnum<PaymentType>(r.payment_type_id.ToString()),
                status = Utilities.CastEnum<PaymentStatus>(r.status.ToString()),
                status_detail = Utilities.CastEnum<PaymentStatusDetails>(r.status_detail.ToString()),
                transaction_amount = r.transaction_amount,
                total_paid_amount = r!.transaction_details!.total_paid_amount,
                transaction_amount_refunded = r.transaction_amount_refunded,
                net_received_amount = r!.transaction_details!.net_received_amount,
                installments = r!.installments
            };
        }
        return new();
    }
    #endregion Fin Payments/pagos
}
