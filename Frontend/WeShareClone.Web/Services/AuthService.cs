using System.Net.Http.Json;
using WeShareClone.Web.Models;

namespace WeShareClone.Web.Services;

/// <summary>Handles the email OTP login flow against the WeShare API.</summary>
public class AuthService(HttpClient http, AuthStateService authState)
{
    /// <summary>
    /// Sends a login request for the given email. The API will email a 6-digit code
    /// if the account exists (silently succeeds otherwise to prevent enumeration).
    /// </summary>
    public async Task RequestLoginAsync(string email)
    {
        await http.PostAsJsonAsync("auth/login", new { email });
    }

    /// <summary>
    /// Verifies the OTP code. Returns <c>true</c> and persists the token on success,
    /// or <c>false</c> if the code is invalid/expired.
    /// </summary>
    public async Task<bool> VerifyCodeAsync(string email, string code)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync("auth/verify", new { email, code });

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        AuthTokenModel? token = await response.Content.ReadFromJsonAsync<AuthTokenModel>();

        if (token is null)
        {
            return false;
        }

        await authState.SetTokenAsync(token.AccessToken, token.RefreshToken);

        return true;
    }

    /// <summary>Uses the stored refresh token to obtain a new access token.</summary>
    public async Task<bool> RefreshAsync()
    {
        if (authState.RefreshToken is null)
        {
            return false;
        }

        HttpResponseMessage response = await http.PostAsJsonAsync(
            "auth/refresh",
            new { refreshToken = authState.RefreshToken }
        );

        if (!response.IsSuccessStatusCode)
        {
            await authState.ClearAsync();
            return false;
        }

        AuthTokenModel? token = await response.Content.ReadFromJsonAsync<AuthTokenModel>();

        if (token is null)
        {
            return false;
        }

        await authState.SetTokenAsync(token.AccessToken, token.RefreshToken);

        return true;
    }
}
