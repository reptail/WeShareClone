namespace WeShareClone.Domain.Models;

public record PasskeyChallenge(
    int Id,
    string Email,
    EPasskeyChallengeType ChallengeType,
    string OptionsJson,
    DateTime ExpiresAtUtc
);