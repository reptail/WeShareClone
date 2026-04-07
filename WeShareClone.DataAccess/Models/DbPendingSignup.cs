namespace WeShareClone.DataAccess.Models;

public class DbPendingSignup
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}