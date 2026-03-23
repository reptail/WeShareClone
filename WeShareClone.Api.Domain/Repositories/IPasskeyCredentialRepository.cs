using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.Domain.Repositories;

public interface IPasskeyCredentialRepository
{
    Task<PasskeyCredential[]> GetByUserIdAsync(int userId);
    Task<PasskeyCredential?> GetByCredentialIdAsync(byte[] credentialId);
    Task<PasskeyCredential> CreateAsync(int userId, byte[] credentialId, byte[] publicKey, long signCount, Guid aaGuid);
    Task UpdateSignCountAsync(int id, long signCount);
    Task DeleteByIdAsync(int id, int userId);
}