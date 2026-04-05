using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IPendingSignupRepository
{
    Task<PendingSignup?> GetByEmailAsync(string email);
    Task UpsertAsync(string email, string name);
    Task DeleteByEmailAsync(string email);
}