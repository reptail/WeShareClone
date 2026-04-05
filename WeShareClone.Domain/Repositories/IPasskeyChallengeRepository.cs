using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IPasskeyChallengeRepository
{
    Task<PasskeyChallenge?> GetByEmailAndTypeAsync(string email, EPasskeyChallengeType type);
    Task UpsertAsync(string email, EPasskeyChallengeType type, string optionsJson, DateTime expiresAtUtc);
    Task DeleteByEmailAndTypeAsync(string email, EPasskeyChallengeType type);
}