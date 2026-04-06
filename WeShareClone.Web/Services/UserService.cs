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
}
