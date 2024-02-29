using Newtonsoft.Json;
using WrapperMercadoPagoAPI.Interface;
using WrapperMercadoPagoAPI.Model;
using WrapperMercadoPagoAPI.Service;

namespace WrapperMercadoPagoAPI.General;
//[ComVisible(true), Guid(ContractGuids.GuidGrwManagerMP), ProgId("WrapperMercadoPagoAPI.GrwManagerMP")]
//[ComDefaultInterface(typeof(IGrwManagerMP))]
public class GrwManagerMP : IGrwManagerMP, IDisposable
{
    public GrwManagerMP() { }
    #region Operaciones con Sucursales/Stores
    Sucursal? IGrwManagerMP.GetSucursalFromName(string name)
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
                id = long.Parse(r.id),
                business_hours = r.business_hours
            };
        }
        return default;
    }
    Sucursales? IGrwManagerMP.GetSucursales(long id, string external_id)
    {
        using SucursalService ss = new();
        var result = Task.Run(() => ss.GetSucursal(id, external_id).Result);
        var r = result.GetAwaiter().GetResult();
        if (r != null)
            return r;
        return default;

    }
    SucursalRequest? IGrwManagerMP.UpdateSucursal(long id, string name, string external_id)
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
            var result = Task.Run(() => ss.UpdateSucurs(id, suc, true).Result);
            return result.GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            ErrorManager.SetErrorMesage(ex.Message, this.ToString());
        }
        return default;
    }
    #endregion Sucursales/Stores
    #region Operaciones con Brunchs/cajas
    BranchRequest? IGrwManagerMP.CreateBranch(string inName, long inStore_id, string inExternal_store_id, string inExternal_id, BranchCategories inCategory, bool inFixed_amount)
    {
        try
        {
            Branch br = new()
            {
                name = inName,
                store_id = inStore_id,
                external_store_id = inExternal_store_id,
                external_id = inExternal_id,
                category = inCategory,
                fixed_amount = inFixed_amount
            };

            using BranchService ss = new();
            JsonSerializerSettings settings = new()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var result = Task.Run(() => ss.AddBranch(br, settings).Result);
            return result.GetAwaiter().GetResult();
        }
        catch { return default; }
    }
    bool IGrwManagerMP.DeleteBrunch(long id)
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
    BranchRequest? IGrwManagerMP.UpdateBrunch(long id, string name, bool fixed_amount, BranchCategories category)
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
            return result.GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            ErrorManager.SetErrorMesage(ex.Message, this.ToString());
            return default;
        }
    }
    object IGrwManagerMP.GetBrunch(long id)
    {
        return new object();
    }
    #endregion Branchs

    #region Operaciones con Ordenes/Orders
    //int AddItemToOrder(double unitPrice, double quantity, double totalAmout, string skuNumber = "", string title = "", string category = "", string description = "", string unitMesure = "");
    int IGrwManagerMP.AddItemToOrder(double unitPrice, double quantity, double totalAmout, string skuNumber, string title, string category, string description, string unitMesure)
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
    bool IGrwManagerMP.CreateSingleItemOrder(string externalStoreID, string externalPosID, double totalAmount, string itemDescription, string itemUnit, double cashOut, string externalReference, string title, string desription, double timeOutSecond)
    {
        try
        {
            using OrderService os = new();
            // format = "yyyy-MM-ddTHH:mm:ss.fff-HH:mm";
            var format = "yyyy-MM-ddTHH:mm:00.000-04:00";
            ItemOrder itemOrder = new()
            {
                title = itemDescription,
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
                expiration_date = DateTime.Now.AddSeconds(timeOutSecond).ToString(format),
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
    bool IGrwManagerMP.CreateListItemOrder(string externalStoreID, string externalPosID, double totalAmount, double cashOut, string externalReference, string title, string desription, double timeOutSecond)
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

    Order? IGrwManagerMP.GetOrder(string externalPosID)
    {
        using OrderService os = new();
        var result = Task.Run(() => os.GetOrder(externalPosID).Result);
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
    Payments? IGrwManagerMP.GetListPayments(DateTime? beginDate, DateTime? endDate, string storeID, string posID, string externalReference)
    {
        using PaymentService os = new();
        var result = Task.Run(() => os.GetPayments(beginDate, endDate, storeID, posID, externalReference).Result);
        return result.GetAwaiter().GetResult();

    }

    PaymentRequest? IGrwManagerMP.GetPayment(string storeID, string posID, string externalReference)
    {
        using PaymentService os = new();
        var result = Task.Run(() => os.GetPayments(null, null, storeID, posID, externalReference).Result);
        var listPayment = result.GetAwaiter().GetResult();
        if (listPayment != null)
            return listPayment!.results.FirstOrDefault();
        return default;
    }

    #endregion Fin Payments/pagos

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _listitemOrder.Clear();
            _listitemOrder = null!;
        }
    }

}
