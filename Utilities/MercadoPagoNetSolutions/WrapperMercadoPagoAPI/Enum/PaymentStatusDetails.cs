﻿namespace WrapperMercadoPagoAPI.Enum;
public enum PaymentStatusDetails
{
    None,
    Accredited,
    pending_contingency,
    pending_review_manual,
    cc_rejected_bad_filled_date,
    cc_rejected_bad_filled_other,
    cc_rejected_bad_filled_security_code,
    cc_rejected_blacklist,
    cc_rejected_call_for_authorize,
    cc_rejected_card_disabled,
    cc_rejected_duplicated_payment,
    cc_rejected_high_risk,
    cc_rejected_insufficient_amount,
    cc_rejected_invalid_installments,
    cc_rejected_max_attempts,
    cc_rejected_other_reason
}
