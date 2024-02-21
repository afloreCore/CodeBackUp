using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperMercadoPagoAPI.Enum;
/*
ARS: Argentine peso.
BRL: Brazilian real.
CLP: Chilean peso.
MXN: Mexican peso.
COP: Colombian peso.
PEN: Peruvian sol.
UYU: Uruguayan peso.
VES: Venezuelan Bolivar.
MCN: Mercado Coin (internal cryptocurrency of Mercado Livre and Mercado Pago).
BTC: Bitcoin.
USD: US dollar.
USDP: Pax Dollar (stable cryptocurrency pegged to the US dollar).
DCE: Dollar Currency Equivalent.
ETH: Ethereum.
FDI: Investment Funds.
CDB: Bank deposit certificate
*/
public enum CurrencyType
{
    None,
    ARS,
    BRL,
    CLP,
    MXN,
    COP,
    PEN,
    UYU,
    VES,
    MCN,
    BTC,
    USD,
    DCE,
    ETH,
    FDI,
    CDB

}

