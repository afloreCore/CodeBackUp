namespace WrapperMercadoPagoAPI.Model;
internal class OrderRequest
{
    public class Item
    {
        public string sku_number { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string unit_measure { get; set; } = string.Empty;
        public int quantity { get; set; }
        public double unit_price { get; set; }
        public double total_amount { get; set; }
    }

    public class Root
    {
        public string external_reference { get; set; } = string.Empty;
        public double total_amount { get; set; }
        public List<Item> items { get; set; } = null!;
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public Sponsor sponsor { get; set; } = null!;
        public CashOut cash_out { get; set; } = null!;
    }


}
