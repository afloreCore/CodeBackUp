namespace WrapperMercadoPagoAPI.Enum;
/*
 Es el tipo de método de pago (tarjeta, transferencia bancaria, ticket, ATM, etc.). Puede ser de los siguientes tipos.
account_money: Money in the Mercado Pago account.
ticket: Boletos, Caixa Electronica Payment, PayCash, Efecty, Oxxo, etc.
bank_transfer: Pix and PSE (Pagos Seguros en Línea).
atm: ATM payment (widely used in Mexico through BBVA Bancomer).
credit_card: Payment by credit card.
debit_card: Payment by debit card.
prepaid_card: Payment by prepaid card.
digital_currency: Purchases with Mercado Crédito.
digital_wallet: Paypal.
voucher_card: Alelo benefits, Sodexo.
crypto_transfer: Payment with cryptocurrencies such as Ethereum and Bitcoin.
*/
public enum PaymentType
{
    None,
    account_money,
    ticket,
    bank_transfer,
    atm,
    credit_card,
    debit_card,
    prepaid_card,
    digital_currency,
    digital_wallet,
    voucher_card,
    crypto_transfer
}
