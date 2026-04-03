using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.Domain.Repositories;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetByEmailAsync(string email);
    Task UpsertAsync(string email, string codeHash, DateTime expiresAtUtc);
    Task DeleteByEmailAsync(string email);
}