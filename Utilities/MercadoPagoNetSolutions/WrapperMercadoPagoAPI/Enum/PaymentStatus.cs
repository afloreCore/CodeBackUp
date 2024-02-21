using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.Enum;
[ComVisible(true)]
public enum PaymentStatus
{
    None,
    pending,
    approved,
    authorized,
    in_process,
    in_mediation,
    rejected,
    cancelled,
    refunded,
    charged_back
}
