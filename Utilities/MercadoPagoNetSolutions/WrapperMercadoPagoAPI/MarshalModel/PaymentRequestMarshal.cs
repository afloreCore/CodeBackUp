using System.Runtime.InteropServices;
using WrapperMercadoPagoAPI.Enum;

namespace WrapperMercadoPagoAPI.MarshalModel;
[ComVisible(true), Guid("F4F4EEF3-BF6D-4790-98A9-A6FDBDEEAF65"), ClassInterface(ClassInterfaceType.AutoDispatch), ProgId("PaymentRequestMarshal")]
public class PaymentRequestMarshal
{
    public double id { get; set; }
    public string authorization_code { get; set; } = string.Empty;
    public bool binary_mode { get; set; }
    public DateTime date_created { get; set; }
    public DateTime date_approved { get; set; }
    public DateTime date_last_updated { get; set; }
    public DateTime money_release_date { get; set; }
    public PaymentMethodID payment_method_id { get; set; }//master-visa-account_money-etc
    public PaymentType payment_type_id { get; set; }
    public PaymentStatus status { get; set; }
    public PaymentStatusDetails status_detail { get; set; }
    public string currency_id { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public double collector_id { get; set; }
    public double transaction_amount { get; set; }
    public double transaction_amount_refunded { get; set; }
    public double coupon_amount { get; set; }
    public double total_paid_amount { get; set; } //from transaction_details
    public double net_received_amount { get; set; } //from transaction_details
    public int installments { get; set; }
    public string external_reference { get; set; } = string.Empty;
    public CardMarshal card { get; set; } = null!;
    //public string car_bin { get; set; } = string.Empty;
    //public DateTime card_date_created { get; set; }
    //public DateTime car_date_last_updated { get; set; }
    //public int car_expiration_month { get; set; }
    //public int car_expiration_year { get; set; }
    //public string car_first_six_digits { get; set; } = string.Empty;
    //public string car_id { get; set; } = string.Empty;
    //public string car_last_four_digits { get; set; } = string.Empty;
}
