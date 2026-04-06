using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IExchangeRateRepository
{
    /// <summary>Returns the current revision of each exchange rate.</summary>
    /// <param name="currencies">Optional filter; when null or empty, all currencies are returned.</param>
    Task<ExchangeRate[]> GetLatestAsync(IEnumerable<string>? currencies = null);

    /// <summary>
    /// For each rate, inserts a new revision only when the <see cref="ExchangeRate.Rate"/>
    /// value has changed since the last stored revision for that currency.
    /// </summary>
    Task InsertManyIfChangedAsync(IEnumerable<ExchangeRate> rates);
}
