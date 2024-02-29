namespace WrapperMercadoPagoAPI.Model;
public class PaymentRequest
{
    public class Accounts
    {
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
    }

    public class Amounts
    {
        public double original { get; set; }
        public int refunded { get; set; }
    }

    public class OrderDetails
    {
        public string id { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
    }
    public class AdditionalInfo
    {
    }

    public class Card
    {
        public string bin { get; set; } = string.Empty;
        public Cardholder cardholder { get; set; } = null!;
        public DateTime? date_created { get; set; }
        public DateTime? date_last_updated { get; set; }
        public int expiration_month { get; set; }
        public int expiration_year { get; set; }
        public string first_six_digits { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public string last_four_digits { get; set; } = string.Empty;
    }

    public class Cardholder
    {
        public Identification identification { get; set; } = null!;
        public string name { get; set; } = string.Empty;
    }

    public class Identification
    {
        public string type { get; set; } = string.Empty;
        public long number { get; set; }
    }
    public class Metadata
    {
    }

    public class RefunCharges
    {

    }
    public class MetadataChargeDetails
    {
        public string name { get; set; } = string.Empty;
        public RefunCharges refund_charges { get; set; } = null!;
        public object reserve_id { get; set; } = null!;
        public string type { get; set; } = string.Empty;
    }

    public class Payer
    {
        public int id { get; set; }
        public string email { get; set; } = string.Empty;
        public Identification identification { get; set; } = null!;
        public string type { get; set; } = string.Empty;
    }

    public class TransactionDetails
    {
        public double net_received_amount { get; set; }
        public double total_paid_amount { get; set; }
        public double overpaid_amount { get; set; }
        public double installment_amount { get; set; }
    }
    public class FeeDetails
    {
        public double amount { get; set; }
        public string fee_payer { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
    }
    public class Expanded
    {
        public Gateway gateway { get; set; } = null!;
    }

    /*
     "gateway": {
                    "authorization_code": "301299",
                    "buyer_fee": 0,
                    "connection": "firstdata-ipg",
                    "date_created": "2023-10-20T13:52:13.000-04:00",
                    "finance_charge": 0,
                    "id": "42397162311_737b736e76737b7a7576",
                    "installments": 1,
                    "issuer_id": "3",
                    "merchant": null,
                    "operation": "purchase",
                    "options": "[{"collector_id":1506231285},{"payment_operation_type":"regular_payment"},{"has_wallet_id":true},
                                {"regulation":
                                    {"legal_name":"Test Test","address_street":"Test Address","address_door_number":123,"zip":"1414","city":"Palermo","country":"ARG",
                                        "document_number":"1111111","document_type":"DNI","region_code":"AR","region_code_iso":"AR-C","fiscal_condition":null,"mcc":"8999"}},
                                {"security_code_data":{"ads_remove_cvv":false}}]",
                    "payment_id": 65617512516,
                    "profile_id": "g2_firstdata-ipg_firstdata_25204885",
                    "reference": "{"merchant_number":30121999}",
                    "soft_descriptor": "QR",
                    "statement_descriptor": "Mercadopago*fake",
                    "usn": null
                }
    */
    public class Gateway
    {
        public string authorization_code { get; set; } = string.Empty;
        public int buyer_fee { get; set; }
        public string connection { get; set; } = string.Empty;
        public DateTime date_created { get; set; }
        public double finance_charge { get; set; }
        public string id { get; set; } = string.Empty;
        public int installments { get; set; }
        public string issuer_id { get; set; } = string.Empty;
        public object merchant { get; set; } = null!;
        public string operation { get; set; } = string.Empty;
        public string options { get; set; } = string.Empty;
        //public List<Option> options { get; set; } = null!;
        public double payment_id { get; set; }
        public string profile_id { get; set; } = null!;
        public string reference { get; set; } = null!;
        public string soft_descriptor { get; set; } = null!;
        public string statement_descriptor { get; set; } = null!;
        public object usn { get; set; } = null!;
    }

    public class Option
    {
        public double collector_id { get; set; }
        public string payment_operation_type { get; set; } = string.Empty;
        public bool? has_wallet_id { get; set; }
        public Regulation regulation { get; set; } = null!;
        public SecurityCodeData security_code_data { get; set; } = null!;
    }

    public class Regulation
    {
        public string legal_name { get; set; } = null!;
        public string address_street { get; set; } = null!;
        public int address_door_number { get; set; }
        public string zip { get; set; } = null!;
        public string city { get; set; } = null!;
        public string country { get; set; } = null!;
        public string document_number { get; set; } = null!;
        public string document_type { get; set; } = null!;
        public string region_code { get; set; } = null!;
        public string region_code_iso { get; set; } = null!;
        public object fiscal_condition { get; set; } = null!;
        public string mcc { get; set; } = null!;
    }

    public class SecurityCodeData
    {
        public bool ads_remove_cvv { get; set; }
    }

    public class Data
    {
        public RoutingData routing_data { get; set; } = null!;
    }

    public class PaymentMethod
    {
        public Data data { get; set; } = null!;
        public string id { get; set; } = null!;
        public string issuer_id { get; set; } = null!;
        public string type { get; set; } = null!;
    }
    public class RoutingData
    {
        public string merchant_account_id { get; set; } = null!;
    }

    public double id { get; set; }
    public string authorization_code { get; set; } = string.Empty;
    public bool binary_mode { get; set; }
    public DateTime? date_created { get; set; }
    public DateTime? date_approved { get; set; }
    public DateTime date_last_updated { get; set; }
    public DateTime? money_release_date { get; set; }
    public PaymentMethod payment_method { get; set; } = null!;
    public string payment_method_id { get; set; } = string.Empty;
    public PaymentType payment_type_id { get; set; }
    public PaymentStatus status { get; set; }
    public PaymentStatusDetails status_detail { get; set; }
    public CurrencyType currency_id { get; set; }
    public string description { get; set; } = string.Empty;
    public double collector_id { get; set; }
    public Payer payer { get; set; } = null!;
    public Metadata metadata { get; set; } = null!;
    public AdditionalInfo additional_info { get; set; } = null!;
    public double transaction_amount { get; set; }
    public double transaction_amount_refunded { get; set; }
    public double coupon_amount { get; set; }
    public TransactionDetails transaction_details { get; set; } = null!;
    public int installments { get; set; }
    public Card card { get; set; } = null!;
    public List<FeeDetails> fee_details { get; set; } = null!;
    public string external_reference { get; set; } = string.Empty;
    public OrderDetails order { get; set; } = null!;
    public Expanded expanded { get; set; } = null!;

}
