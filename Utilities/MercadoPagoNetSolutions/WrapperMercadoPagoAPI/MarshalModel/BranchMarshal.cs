using System.ComponentModel;
using System.Runtime.InteropServices;
using WrapperMercadoPagoAPI.Enum;

namespace WrapperMercadoPagoAPI.MarshalModel;
[ComVisible(true), Guid("8C2B8546-DA94-4325-ABEF-9CB469BE0602"), ClassInterface(ClassInterfaceType.AutoDispatch), ProgId("BranchMarshal")]
public class BranchMarshal
{
    //Se colocó null ya que para el alta o update al Serializar se debe excluir, si es cero 
    //no se excluye porque los DefaultValues no se ignoran en este caso, por ejemplo para 
    //fixed_amount que es false
    public long id { get; set; } 
    public string name { get; set; } = string.Empty;
    public bool fixed_amount { get; set; }
    public long store_id { get; set; }
    public string external_store_id { get; set; } = string.Empty;
    public string external_id { get; set; } = string.Empty;
    public BranchCategories category { get; set; }
}
