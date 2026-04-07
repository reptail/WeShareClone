using System.Net.Http.Json;
using WeShareClone.Web.Models;

namespace WeShareClone.Web.Services;

/// <summary>API client for user-related endpoints.</summary>
public class UserService(HttpClient http)
{
    /// <summary>
    /// Searches for users by name or email (partial match, up to 10 results).
    /// Returns an empty array if no matches or on failure.
    /// </summary>
    public async Task<UserModel[]> SearchAsync(string query)
    {
        try
        {
            return await http.GetFromJsonAsync<UserModel[]>(
                $"api/users/search?q={Uri.EscapeDataString(query)}"
            ) ?? [];
        }
        catch
        {
            return [];
        }
    }

    /// <summary>
    /// Returns the authenticated user's own profile.
    /// Returns <c>null</c> on failure.
    /// </summary>
    public async Task<UserModel?> GetMeAsync()
    {
        try
        {
            return await http.GetFromJsonAsync<UserModel>("api/users/me");
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Updates the authenticated user's profile (name and phone number).
    /// Returns <c>true</c> on success.
    /// </summary>
    public async Task<bool> UpdateProfileAsync(string name, string? phone = null)
    {
        HttpResponseMessage response = await http.PutAsJsonAsync(
            "api/users/me",
            new { name, phone }
        );
        return response.IsSuccessStatusCode;
    }
}
