namespace WeShareClone.Dto.Auth;

public class AuthTokenDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }

    public AuthTokenDto() { }

    public AuthTokenDto(string accessToken, string refreshToken, DateTime expiresAtUtc)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresAtUtc = expiresAtUtc;
    }
}