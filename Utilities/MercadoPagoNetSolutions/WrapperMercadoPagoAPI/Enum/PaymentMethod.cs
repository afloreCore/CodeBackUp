namespace WrapperMercadoPagoAPI.Enum;
/*
 payment_method_id String
Identificador del medio de pago. Indica el ID del medio de pago seleccionado para realizar el pago.
pix: Instant digital payment method used in Brazil.
account_money: When the payment is debited directly from a Mercado Pago account.
debin_transfer: Digital payment method used in Argentina that immediately debits an amount from an account, requesting prior authorization.
ted: It is the Electronic Transfer Available payment, used in Brazil, that has fees to be used. The payment is made the same day of the transaction, but for this it is necessary to make the transfer within the stipulated period.
cvu: Payment method used in Argentina;
pse: Digital payment method used in Colombia which users authorize the debit of funds from their bank through virtual banking. your savings, checking or electronic deposit accounts.
*/
public enum PaymentMethod
{
    None,
    account_money,
    cvu,
    debin_transfer,
    pix,
    pse,
    ted
}
