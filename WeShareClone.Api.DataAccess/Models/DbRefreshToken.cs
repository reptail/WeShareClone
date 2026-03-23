namespace WeShareClone.Api.DataAccess.Models;

public class DbRefreshToken
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
}