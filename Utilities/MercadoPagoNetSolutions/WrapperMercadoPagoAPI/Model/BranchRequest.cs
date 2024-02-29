using System.ComponentModel.DataAnnotations;

namespace WrapperMercadoPagoAPI.Model;
public class BranchRequest
{

    public class Qr
    {
        public string image { get; set; } = string.Empty;
        public string template_document { get; set; } = string.Empty;
        public string template_image { get; set; } = string.Empty;
    }

    [Required]
    public int id { get; set; }
    public Qr? qr { get; set; } = null!;
    public string status { get; set; } = string.Empty;
    public DateTime date_created { get; set; }
    public DateTime date_last_updated { get; set; }
    public string uuid { get; set; } = string.Empty;
    public long user_id { get; set; }
    public string name { get; set; } = string.Empty;
    public bool fixed_amount { get; set; }
    public BranchCategories category { get; set; }
    public string store_id { get; set; } = string.Empty;
    public string external_store_id { get; set; } = string.Empty;
    public string external_id { get; set; } = string.Empty;
    public string site { get; set; } = string.Empty;
    public string qr_code { get; set; } = string.Empty;

}
