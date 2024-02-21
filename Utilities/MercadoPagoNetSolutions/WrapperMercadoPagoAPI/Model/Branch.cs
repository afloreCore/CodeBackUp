using System.ComponentModel;
using WrapperMercadoPagoAPI.Enum;

namespace WrapperMercadoPagoAPI.Model;
public class Branch
{
    //Se colocó null ya que para el alta o update al Serializar se debe excluir, si es cero 
    //no se excluye porque los DefaultValues no se ignoran en este caso, por ejemplo para 
    //fixed_amount que es false
    [DefaultValue(0)]
    public long id { get; set; }
    [DefaultValue("")]
    public string name { get; set; } = string.Empty;

    [DefaultValue(false)]
    public bool fixed_amount { get; set; }
    [DefaultValue(0)]
    public long store_id { get; set; }
    [DefaultValue("")]
    public string external_store_id { get; set; } = string.Empty;
    [DefaultValue("")]
    public string external_id { get; set; } = string.Empty;

    [DefaultValue(BranchCategories.None)]
    public BranchCategories category { get; set; }
}
