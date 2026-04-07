using System.Net.Http.Json;
using WeShareClone.Web.Models;

namespace WeShareClone.Web.Services;

/// <summary>API client for settlement-related endpoints.</summary>
public class SettlementService(HttpClient http)
{
    /// <summary>Returns all settlements the current user is a member or creator of.</summary>
    public async Task<SettlementModel[]> GetMySettlementsAsync()
        => await http.GetFromJsonAsync<SettlementModel[]>("api/settlements/my") ?? [];

    /// <summary>Returns a single settlement by ID.</summary>
    public async Task<SettlementModel?> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<SettlementModel>($"api/settlements/{id}");

    /// <summary>Creates a new settlement. Returns the created settlement.</summary>
    public async Task<SettlementModel?> CreateAsync(string name, string currency)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync("api/settlements", new
        {
            name,
            currency,
        });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SettlementModel>();
    }

    /// <summary>Returns all participants (users) in a settlement.</summary>
    public async Task<UserModel[]> GetParticipantsAsync(int settlementId)
        => await http.GetFromJsonAsync<UserModel[]>($"api/settlements/{settlementId}/participants") ?? [];

    /// <summary>Adds a user to a settlement by user ID. Returns true on success.</summary>
    public async Task<bool> AddUserAsync(int settlementId, int userId)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync(
            $"api/settlements/{settlementId}/users",
            new { userId }
        );

        return response.IsSuccessStatusCode;
    }

    /// <summary>Starts settling: transitions settlement to BeingSettled and returns persisted debts.</summary>
    public async Task<SettlementDebtModel[]?> StartSettlingAsync(int settlementId)
    {
        HttpResponseMessage response = await http.PostAsync($"api/settlements/{settlementId}/settle", null);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<SettlementDebtModel[]>();
    }

    /// <summary>Reverts settlement from BeingSettled to Open, deleting all debt records.</summary>
    public async Task<bool> RevertToOpenAsync(int settlementId)
    {
        HttpResponseMessage response = await http.DeleteAsync($"api/settlements/{settlementId}/settle");
        return response.IsSuccessStatusCode;
    }

    /// <summary>Closes a settlement. All debts must already be paid.</summary>
    public async Task<bool> CloseAsync(int settlementId)
    {
        HttpResponseMessage response = await http.PostAsync($"api/settlements/{settlementId}/close", null);
        return response.IsSuccessStatusCode;
    }

    /// <summary>Reopens a closed settlement, deleting all debt records.</summary>
    public async Task<bool> ReopenAsync(int settlementId)
    {
        HttpResponseMessage response = await http.PostAsync($"api/settlements/{settlementId}/reopen", null);
        return response.IsSuccessStatusCode;
    }

    /// <summary>Returns all debts for a settlement.</summary>
    public async Task<SettlementDebtModel[]> GetDebtsAsync(int settlementId)
        => await http.GetFromJsonAsync<SettlementDebtModel[]>($"api/settlements/{settlementId}/debts") ?? [];

    /// <summary>Marks a debt as paid or unpaid. Returns the updated debt.</summary>
    public async Task<SettlementDebtModel?> MarkDebtPaidAsync(int settlementId, int debtId, bool isPaid)
    {
        HttpResponseMessage response = await http.PatchAsJsonAsync(
            $"api/settlements/{settlementId}/debts/{debtId}",
            isPaid
        );
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<SettlementDebtModel>();
    }
}

