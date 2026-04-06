using WeShareClone.Web.Models;

namespace WeShareClone.Web.Utilities;

/// <summary>
/// Converts monetary values between currencies using Danmarks Nationalbank exchange rates.
/// Rates are expressed as DKK per 100 units of the target currency; DKK itself has an implicit rate of 100.
/// </summary>
public static class CurrencyConverter
{
    private const decimal DkkRate = 100m;

    /// <summary>
    /// Converts <paramref name="value"/> from <paramref name="fromCurrency"/> to <paramref name="toCurrency"/>.
    /// Returns <paramref name="value"/> unchanged if the currencies are equal or if a required rate is missing.
    /// </summary>
    public static decimal Convert(
        decimal value,
        string fromCurrency,
        string toCurrency,
        ExchangeRateModel[] rates)
    {
        if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            return value;

        decimal fromRate = GetRate(fromCurrency, rates);
        decimal toRate   = GetRate(toCurrency,   rates);

        if (fromRate == 0 || toRate == 0)
            return value;

        return value * fromRate / toRate;
    }

    private static decimal GetRate(string currency, ExchangeRateModel[] rates)
    {
        if (string.Equals(currency, CurrencyCodes.DKK, StringComparison.OrdinalIgnoreCase))
            return DkkRate;

        ExchangeRateModel? match = Array.Find(
            rates,
            r => string.Equals(r.Currency, currency, StringComparison.OrdinalIgnoreCase)
        );

        return match?.Rate ?? 0m;
    }
}
