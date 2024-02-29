using WrapperMercadoPagoAPI.Model;

namespace WrapperMercadoPagoAPI.Interface;
//[ComVisible(true), Guid(ContractGuids.GuidIGrwManagerMP), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IGrwManagerMP
{
    #region Operaciones con Sucuresales
    Sucursal? GetSucursalFromName(string name);
    Sucursales? GetSucursales(long id = 0, string external_id = "");
    /// <summary>
    /// Modifica campos de las sucursales dadas de alta. Para modificar: business_hours y location, deberá realizarse en el sitio de MP 
    /// </summary>
    /// <param name="originalName">Descripción con que se creó la sucursal</param>
    /// <param name="newName">Nueva descripción de la sucursal</param>
    /// <param name="external_id">Nuevo identidficador externo</param>
    /// <returns>String conteniendo estructura JSON con todos los datos del elemento modificado</returns>
    SucursalRequest? UpdateSucursal(long id, string name = "", string external_id = "");
    #endregion Fin Sucursales
    #region Operaciones con Cajas/Brunchs
    /*
     * name": "First POS",
      "fixed_amount": false,
      "store_id": 1234567,
      "external_store_id": "SUC001",
      "external_id": "SUC001POS001",
      "category": 621102
    */
    /// <summary>
    /// /
    /// </summary>
    /// <param name="name">Descripción del branch/caja</param>
    /// <param name="store_id">Identificador de la sucursal relacionada</param>
    /// <param name="external_store_id">Código de identificación alfanumérico</param>
    /// <param name="category">Código MCC que indica el rubro del Punto de Venta. Si no se especifica, queda como categoría genérica</param>
    /// <param name="fixed_amount">Determina si el cliente puede insertar el monto a pagar. Default: false</param>
    /// <returns>String conteniendo estructura JSON con todos los datos del nuevo elemento</returns>
    BranchRequest? CreateBranch(string name, long store_id, string external_store_id, string external_idB, BranchCategories category = BranchCategories.None, bool fixed_amount = false);

    BranchRequest? UpdateBrunch(long id, string name = "", bool fixed_amount = false, BranchCategories category = BranchCategories.None);
    bool DeleteBrunch(long id);

    object GetBrunch(long id);
    #endregion Fin Cajas/Brunchs
    #region Operaciones con Ordenes/Orders

    int AddItemToOrder(double unitPrice, double quantity, double totalAmout, string skuNumber = "", string title = "", string category = "", string description = "", string unitMesure = "");
    bool CreateListItemOrder(string externalStoreID, string externalPosID, double totalAmount, double cashOut = 0, string externalReference = "", string title = "", string desription = "", double timeOutSecond = 0);
    /// <summary>
    /// Create single order with one default item 
    /// </summary>
    /// <param name="externalStoreID">Requerid</param>
    /// <param name="externalPosID">Requerid</param>
    /// <param name="totalAmount">Total Amount from tiket</param>
    /// <param name="itemDescription">Default item description</param>
    /// <param name="itemUnit">Unit description</param>
    /// <param name="cashOut">Cash amount for extraction</param>
    /// <param name="externalReference">UniquetTicket referencia code</param>
    /// <param name="title">option</param>
    /// <param name="desription">option</param>
    /// <param name="timeOutSecond">Seconds extra time for expire</param>
    /// <returns></returns>
    bool CreateSingleItemOrder(string externalStoreID, string externalPosID, double totalAmount, string itemDescription, string itemUnit, double cashOut = 0, string externalReference = "", string title = "", string desription = "", double timeOutSecond = 0);
    Order? GetOrder(string externalPosID);
    Payments? GetListPayments(DateTime? beginDate, DateTime? endDate, string storeID = "", string posID = "", string externalReference = "");
    PaymentRequest? GetPayment(string storeID, string posID, string externalReference);

    #endregion Ordenes/orders


}
