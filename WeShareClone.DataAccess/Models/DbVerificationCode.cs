namespace WeShareClone.DataAccess.Models;

public class DbVerificationCode
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string CodeHash { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
}