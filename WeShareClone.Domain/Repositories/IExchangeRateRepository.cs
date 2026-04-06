using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IExchangeRateRepository
{
    /// <summary>Returns all exchange rates for the most recent available closing date.</summary>
    Task<ExchangeRate[]> GetLatestAsync();

    /// <summary>Returns all exchange rates for the specified closing date.</summary>
    Task<ExchangeRate[]> GetByDateAsync(DateOnly date);

    /// <summary>
    /// Inserts or updates exchange rates. Existing records for the same (Date, Currency)
    /// pair are overwritten.
    /// </summary>
    Task UpsertManyAsync(IEnumerable<ExchangeRate> rates);
}
