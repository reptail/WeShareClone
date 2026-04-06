namespace WeShareClone.Dto.ExchangeRates;

/// <param name="Date">The closing day the rate was published for.</param>
/// <param name="Currency">ISO 4217 currency code (target; base is always DKK).</param>
/// <param name="Rate">Cost in DKK per 100 units of the target currency.</param>
public record ExchangeRateDto(
    DateOnly Date,
    string Currency,
    decimal Rate
);
