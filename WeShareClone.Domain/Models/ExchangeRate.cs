namespace WeShareClone.Domain.Models;

/// <param name="Date">The closing day the rate is published for.</param>
/// <param name="Currency">ISO 4217 currency code (target; base is always DKK).</param>
/// <param name="Rate">Cost in DKK per 100 units of the target currency.</param>
/// <param name="InsertedAtUtc">When the record was inserted into the database.</param>
public record ExchangeRate(
    DateOnly Date,
    string Currency,
    decimal Rate,
    DateTime InsertedAtUtc
);
