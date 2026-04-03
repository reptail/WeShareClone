using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.Domain.Repositories;

public interface IPasskeyChallengeRepository
{
    Task<PasskeyChallenge?> GetByEmailAndTypeAsync(string email, EPasskeyChallengeType type);
    Task UpsertAsync(string email, EPasskeyChallengeType type, string optionsJson, DateTime expiresAtUtc);
    Task DeleteByEmailAndTypeAsync(string email, EPasskeyChallengeType type);
}