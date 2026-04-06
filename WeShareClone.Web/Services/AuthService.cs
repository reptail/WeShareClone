using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
        await http.PostAsJsonAsync("api/auth/login", new { email });
    }

    /// <summary>
    /// Verifies the OTP code. Returns <c>true</c> and persists the token on success,
    /// or <c>false</c> if the code is invalid/expired.
    /// </summary>
    public async Task<bool> VerifyCodeAsync(string email, string code)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync("api/auth/verify", new { email, code });

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
            "api/auth/refresh",
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

    // -------------------------------------------------------------------------
    // Sign-up
    // -------------------------------------------------------------------------

    /// <summary>
    /// Initiates sign-up for a new account. The API sends a 6-digit code to the
    /// provided email address. Pass <paramref name="inviteToken"/> when the API is
    /// configured for invite-only sign-up.
    /// </summary>
    public async Task SignupAsync(string email, string name, string? inviteToken = null)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync(
            "api/auth/signup",
            new { email, name, inviteToken }
        );
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Verifies the sign-up OTP code. Returns <c>true</c> and persists the token on
    /// success, or <c>false</c> if the code is invalid/expired.
    /// </summary>
    public async Task<bool> VerifySignupAsync(string email, string code)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync("api/auth/signup/verify", new { email, code });

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

    // -------------------------------------------------------------------------
    // Passkey login
    // -------------------------------------------------------------------------

    /// <summary>
    /// Begins a passkey login for the given email. Returns the raw
    /// <c>AssertionOptions</c> JSON string from the server, or <c>null</c> if no
    /// passkeys are registered for this email.
    /// </summary>
    public async Task<string?> PasskeyBeginLoginAsync(string email)
    {
        HttpResponseMessage response = await http.PostAsJsonAsync(
            "api/auth/passkey/login/begin",
            new { email }
        );

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Completes a passkey login by posting the browser's assertion response JSON.
    /// Returns <c>true</c> and persists the token on success.
    /// </summary>
    public async Task<bool> PasskeyCompleteLoginAsync(string assertionJson)
    {
        using StringContent content = new(assertionJson, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await http.PostAsync("api/auth/passkey/login/complete", content);

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

    // -------------------------------------------------------------------------
    // Passkey registration
    // -------------------------------------------------------------------------

    /// <summary>
    /// Begins passkey registration for the currently authenticated user.
    /// Returns the raw <c>CredentialCreateOptions</c> JSON string from the server.
    /// </summary>
    public async Task<string?> PasskeyRegisterBeginAsync()
    {
        HttpResponseMessage response = await http.PostAsync("api/auth/passkey/register/begin", null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Completes passkey registration by posting the browser's attestation response JSON.
    /// Returns <c>true</c> on success (204 NoContent).
    /// </summary>
    public async Task<bool> PasskeyRegisterCompleteAsync(string attestationJson)
    {
        using StringContent content = new(attestationJson, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await http.PostAsync("api/auth/passkey/register/complete", content);

        return response.IsSuccessStatusCode;
    }
}
