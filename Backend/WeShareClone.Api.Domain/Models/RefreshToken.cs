namespace WeShareClone.Api.Domain.Models;

public record RefreshToken(
    int Id,
    int UserId,
    string Token,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc);