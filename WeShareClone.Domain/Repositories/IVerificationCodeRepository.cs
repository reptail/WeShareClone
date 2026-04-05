using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetByEmailAsync(string email);
    Task UpsertAsync(string email, string codeHash, DateTime expiresAtUtc);
    Task DeleteByEmailAsync(string email);
}