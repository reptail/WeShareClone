using System.Net.Http.Json;
using WeShareClone.Web.Models;

namespace WeShareClone.Web.Services;

public class ExchangeRateService(HttpClient http)
{
    public async Task<ExchangeRateModel[]> GetLatestAsync()
    {
        try
        {
            return await http.GetFromJsonAsync<ExchangeRateModel[]>("api/exchange-rates") ?? [];
        }
        catch
        {
            return [];
        }
    }
}
