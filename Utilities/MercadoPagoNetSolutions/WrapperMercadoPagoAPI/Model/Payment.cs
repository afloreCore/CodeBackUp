using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperMercadoPagoAPI.Model;
public class AdditionalInfo
{
}

public class Card
{
}

public class Identification
{
    public string type { get; set; } = string.Empty;
    public long number { get; set; }
}

public class Metadata
{
}

public class Payer
{
    public int id { get; set; }
    public string email { get; set; } = string.Empty;
    public Identification identification { get; set; } = null!;
    public string type { get; set; } = string.Empty;
}

public class Payment
{
    public int id { get; set; }
    public DateTime date_created { get; set; }
    public DateTime date_approved { get; set; }
    public DateTime date_last_updated { get; set; }
    public DateTime money_release_date { get; set; }
    public string payment_method_id { get; set; } = string.Empty;   
    public string payment_type_id { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public string status_detail { get; set; } = string.Empty;
    public string currency_id { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public int collector_id { get; set; }
    public Payer payer { get; set; } = null!;
    public Metadata metadata { get; set; } = null!;
    public AdditionalInfo additional_info { get; set; } = null!;
    public int transaction_amount { get; set; }
    public int transaction_amount_refunded { get; set; }
    public int coupon_amount { get; set; }
    public TransactionDetails transaction_details { get; set; } = null !;
    public int installments { get; set; }
    public Card card { get; set; } = null!;
}

public class TransactionDetails
{
    public int net_received_amount { get; set; }
    public int total_paid_amount { get; set; }
    public int overpaid_amount { get; set; }
    public int installment_amount { get; set; }
}

