using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WeShareClone.Web.Services;

/// <summary>
/// Singleton that holds the current user's authentication state (JWT + parsed claims).
/// Persists the access token to localStorage and exposes a change notification event.
/// </summary>
public class AuthStateService(LocalStorageService localStorage)
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? AccessTokenExpiresAtUtc { get; private set; }
    public int UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string UserEmail { get; private set; } = string.Empty;
    public bool IsAdmin { get; private set; }
    public bool IsAuthenticated => AccessToken is not null;

    /// <summary>Raised whenever auth state changes so components can re-render.</summary>
    public event Action? OnChange;

    /// <summary>Loads tokens from localStorage and parses claims. Call once on app start.</summary>
    public async Task InitializeAsync()
    {
        string? token = await localStorage.GetItemAsync(AccessTokenKey);
        string? refresh = await localStorage.GetItemAsync(RefreshTokenKey);

        if (token is not null)
        {
            ApplyToken(token, refresh);
        }
    }

    /// <summary>Stores tokens and parses JWT claims.</summary>
    public async Task SetTokenAsync(string accessToken, string refreshToken)
    {
        await localStorage.SetItemAsync(AccessTokenKey, accessToken);
        await localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
        ApplyToken(accessToken, refreshToken);
        OnChange?.Invoke();
    }

    /// <summary>Clears tokens from memory and localStorage.</summary>
    public async Task ClearAsync()
    {
        await localStorage.RemoveItemAsync(AccessTokenKey);
        await localStorage.RemoveItemAsync(RefreshTokenKey);
        AccessToken = null;
        RefreshToken = null;
        AccessTokenExpiresAtUtc = null;
        UserId = 0;
        UserName = string.Empty;
        UserEmail = string.Empty;
        IsAdmin = false;
        OnChange?.Invoke();
    }

    private void ApplyToken(string accessToken, string? refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwt = handler.ReadJwtToken(accessToken);

        string? sub = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value
                   ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        string? name = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value
                    ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        string? email = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value
                     ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        UserId = int.TryParse(sub, out int id) ? id : 0;
        UserName = name ?? string.Empty;
        UserEmail = email ?? string.Empty;
        AccessTokenExpiresAtUtc = jwt.ValidTo == DateTime.MinValue ? null : jwt.ValidTo;

        string? role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        IsAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
    }
}
