using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WrapperMercadoPagoAPI.Enum;
using WrapperMercadoPagoAPI.MarshalModel;
using WrapperMercadoPagoAPI.Model;
using WrapperMercadoPagoAPI.Service;
using InterfaceComNetCore;

namespace WrapperMercadoPagoAPI.Interface;
[ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("10DFB0D7-49D2-4141-A089-6FC5324BA6A0")]
public interface IWrapperMarshal
{
    public ICallbackInterop ReturnValue { get; set; }
    public SucursalRequestMarshal? GetSucursalFromName(string name);
    public SucursalRequestMarshal? GetSucursalFromId(long sucId);
    public SucursalesMarshal? GetSucursales(long id, string external_id);
    public SucursalRequestMarshal? UpdateSucursal(long id, string name, string external_id);
    public BranchRequestMarshal? CreateBranch(long storeId, string externalId, string nameInput = "", bool fixedAmount = false, BranchCategories inCategory = BranchCategories.None);
    public object?[] GetBranchs(long storeId, string externalId = "", string extStoreid = "", long posid = 0);
    public BranchRequestMarshal? UpdateBrunch(long id, string name, bool fixed_amount, BranchCategories category);
    public bool CreateSingleItemOrder(string externalStoreID, string externalPosID, double totalAmount, string itemDescription, string itemUnit, DateTime expidateDateTime, string notificationUrl = "", double cashOut = 0, string externalReference = "", string title = "", string desription = "");
    public Order? GetOrder(string externalPosID);
    OrderMarshal? GetOrderFromId(string orderID);
    public bool CancelOrder(string externalPosID);
    //public PaymentsMarshal? GetListPayments(DateTime? beginDate, DateTime? endDate, string storeID = "", string posID = "", string externalReference = "");
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)]
    public object[] GetListPayments(DateTime? beginDate, DateTime? endDate, string storeID = "", string posID = "", string externalReference = "");
    public PaymentRequestMarshal? GetPayment(string storeID, string posID, string externalReference);
    PaymentRequestMarshal? GetPaymentFromId(string paymentId);
    public PaymentRequestMarshal? GetAndWaitPayment(string storeID, string posID, string externalReference, long timeout, PaymentStatus paystatus = PaymentStatus.None);
    /**********************************************************************************************************************************/
    /*                                                          UTILITIES
    /**********************************************************************************************************************************/
    public string GetStrDateISO8601(DateTime date);
    public DateTime GetDateTimeFromISO8601(string date);
}   