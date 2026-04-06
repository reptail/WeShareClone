namespace WeShareClone.Domain.Models;

/// <param name="Date">The closing day the rate is published for (informational).</param>
/// <param name="Currency">ISO 4217 currency code (target; base is always DKK).</param>
/// <param name="Rate">Cost in DKK per 100 units of the target currency.</param>
/// <param name="ValidFromUtc">When this revision became effective.</param>
/// <param name="ValidToUtc">When this revision was superseded. '9999-12-31' sentinel = current revision.</param>
public record ExchangeRate(
    DateOnly Date,
    string Currency,
    decimal Rate,
    DateTime ValidFromUtc,
    DateTime ValidToUtc
);
