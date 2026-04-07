using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IPendingSignupRepository
{
    Task<PendingSignup?> GetByEmailAsync(string email);
    Task UpsertAsync(string email, string name, string? phone = null);
    Task DeleteByEmailAsync(string email);
}