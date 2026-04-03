using System.Net.Http.Json;
using WeShareClone.Web.Models;

namespace WeShareClone.Web.Services;

/// <summary>API client for settlement-related endpoints.</summary>
public class SettlementService(HttpClient http)
{
    /// <summary>Returns all settlements the current user is a member or creator of.</summary>
    public async Task<SettlementModel[]> GetMySettlementsAsync()
        => await http.GetFromJsonAsync<SettlementModel[]>("settlements/my") ?? [];

    /// <summary>Returns a single settlement by ID.</summary>
    public async Task<SettlementModel?> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<SettlementModel>($"settlements/{id}");

    /// <summary>Creates a new settlement. Returns the created settlement.</summary>
    public async Task<SettlementModel?> CreateAsync(string name, string currency, bool isOpen = true)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync("settlements", new
        {
            name,
            currency,
            isOpen
        });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SettlementModel>();
    }

    /// <summary>Returns all participants (users) in a settlement.</summary>
    public async Task<UserModel[]> GetParticipantsAsync(int settlementId)
        => await http.GetFromJsonAsync<UserModel[]>($"settlements/{settlementId}/participants") ?? [];
}
