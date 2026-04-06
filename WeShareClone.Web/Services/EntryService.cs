using System.Net.Http.Json;
using WeShareClone.Web.Models;

namespace WeShareClone.Web.Services;

/// <summary>API client for entry-related endpoints.</summary>
public class EntryService(HttpClient http)
{
    /// <summary>Returns all entries for the given settlement, ordered by date descending.</summary>
    public async Task<EntryModel[]> GetEntriesAsync(int settlementId)
        => await http.GetFromJsonAsync<EntryModel[]>($"api/settlements/{settlementId}/entries") ?? [];

    /// <summary>
    /// Creates a new entry in the given settlement.
    /// <paramref name="distributions"/> holds per-user factors whose semantics depend on
    /// <paramref name="distributionMode"/>:
    /// <list type="bullet">
    ///   <item>EvenSplit — all factors must be equal (value is irrelevant)</item>
    ///   <item>Percentage — factors must sum to 1.0</item>
    ///   <item>FixedAmount — factors must sum to <paramref name="value"/></item>
    /// </list>
    /// </summary>
    public async Task<EntryModel?> CreateAsync(
        int settlementId,
        string name,
        decimal value,
        string currency,
        string distributionMode,
        EntryDistributionModel[] distributions)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync(
            $"api/settlements/{settlementId}/entries",
            new
            {
                name,
                value,
                currency,
                distributionMode,
                distributions = distributions.Select(d => new { d.UserId, d.Factor })
            }
        );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<EntryModel>();
    }

    /// <summary>
    /// Updates an existing entry. Distribution factor semantics are the same as <see cref="CreateAsync"/>.
    /// </summary>
    public async Task<EntryModel?> UpdateAsync(
        int settlementId,
        int entryId,
        string name,
        decimal value,
        string currency,
        string distributionMode,
        EntryDistributionModel[] distributions)
    {
        HttpResponseMessage response = await http.PutAsJsonAsync(
            $"api/settlements/{settlementId}/entries/{entryId}",
            new
            {
                name,
                value,
                currency,
                distributionMode,
                distributions = distributions.Select(d => new { d.UserId, d.Factor })
            }
        );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<EntryModel>();
    }
}
