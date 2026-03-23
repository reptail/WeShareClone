using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.Domain.Repositories;

public interface IPendingSignupRepository
{
    Task<PendingSignup?> GetByEmailAsync(string email);
    Task UpsertAsync(string email, string name);
    Task DeleteByEmailAsync(string email);
}