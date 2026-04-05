using WeShareClone.Domain.Models;

namespace WeShareClone.DataAccess.Models;

public class DbPasskeyChallenge
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public EPasskeyChallengeType ChallengeType { get; init; }
    public string OptionsJson { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}