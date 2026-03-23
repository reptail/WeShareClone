namespace WeShareClone.Api.Domain.Models;

public record AuthToken(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc);