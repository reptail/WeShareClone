using WeShareClone.Domain.Models;

namespace WeShareClone.Services;

public interface IExchangeRateService
{
    /// <summary>
    /// Fetches the latest exchange rates from Danmarks Nationalbank, persists them,
    /// and returns the stored rates.
    /// </summary>
    Task<ExchangeRate[]> FetchAndStoreAsync();
}
