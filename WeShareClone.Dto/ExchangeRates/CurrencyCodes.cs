namespace WeShareClone.Dto.ExchangeRates;

/// <summary>
/// ISO 4217 currency codes available from Danmarks Nationalbank.
/// DKK is the reference (base) currency; all other rates express the cost in DKK per 100 units.
/// </summary>
public static class CurrencyCodes
{
    /// <summary>Danish krone — reference currency.</summary>
    public const string DKK = "DKK";

    public const string AUD = "AUD";
    public const string BRL = "BRL";
    public const string CAD = "CAD";
    public const string CHF = "CHF";
    public const string CNY = "CNY";
    public const string CZK = "CZK";
    public const string EUR = "EUR";
    public const string GBP = "GBP";
    public const string HKD = "HKD";
    public const string HUF = "HUF";
    public const string IDR = "IDR";
    public const string ILS = "ILS";
    public const string INR = "INR";
    public const string ISK = "ISK";
    public const string JPY = "JPY";
    public const string KRW = "KRW";
    public const string MXN = "MXN";
    public const string MYR = "MYR";
    public const string NOK = "NOK";
    public const string NZD = "NZD";
    public const string PHP = "PHP";
    public const string PLN = "PLN";
    public const string RON = "RON";
    public const string SEK = "SEK";
    public const string SGD = "SGD";
    public const string THB = "THB";
    public const string TRY = "TRY";
    public const string USD = "USD";

    /// <summary>South African rand.</summary>
    public const string ZAR = "ZAR";

    /// <summary>Special Drawing Rights (IMF calculated rate).</summary>
    public const string XDR = "XDR";
}
