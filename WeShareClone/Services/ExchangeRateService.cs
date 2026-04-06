using System.Globalization;
using System.Xml.Linq;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.Services;

/// <summary>
/// Fetches exchange rates from Danmarks Nationalbank's XML feed, parses the response,
/// and persists the rates to the database.
/// </summary>
public class ExchangeRateService(
    IHttpClientFactory httpClientFactory,
    IExchangeRateRepository exchangeRateRepository) : IExchangeRateService
{
    private const string XmlFeedUrl = "currencyratesxml?lang=da";

    public async Task<ExchangeRate[]> FetchAndStoreAsync()
    {
        HttpClient client = httpClientFactory.CreateClient("Nationalbanken");
        string xml = await client.GetStringAsync(XmlFeedUrl);

        ExchangeRate[] rates = ParseRates(xml);

        await exchangeRateRepository.InsertManyIfChangedAsync(rates);

        return rates;
    }

    private static ExchangeRate[] ParseRates(string xml)
    {
        XDocument doc = XDocument.Parse(xml);

        XElement dailyRates = doc.Root?.Element("dailyrates")
            ?? throw new InvalidOperationException("Missing <dailyrates> element in exchange rate XML.");

        string closingDateRaw = dailyRates.Attribute("id")?.Value
            ?? throw new InvalidOperationException("Missing id attribute on <dailyrates> element.");

        DateOnly closingDate = DateOnly.Parse(closingDateRaw, CultureInfo.InvariantCulture);

        DateTime fetchedAtUtc = DateTime.UtcNow;
        DateTime maxValidTo = new(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
        CultureInfo danishCulture = CultureInfo.GetCultureInfo("da-DK");

        return dailyRates
            .Elements("currency")
            .Select(el =>
            {
                string code = el.Attribute("code")?.Value
                    ?? throw new InvalidOperationException("Missing code attribute on <currency> element.");

                string rateRaw = el.Attribute("rate")?.Value
                    ?? throw new InvalidOperationException($"Missing rate attribute on <currency code=\"{code}\">.");

                decimal rate = decimal.Parse(rateRaw, danishCulture);

                return new ExchangeRate(
                    Date: closingDate,
                    Currency: code,
                    Rate: rate,
                    ValidFromUtc: fetchedAtUtc,
                    ValidToUtc: maxValidTo
                );
            })
            .ToArray();
    }
}
