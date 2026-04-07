using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IPasskeyCredentialRepository
{
    Task<PasskeyCredential[]> GetByUserIdAsync(int userId);
    Task<PasskeyCredential?> GetByCredentialIdAsync(byte[] credentialId);
    Task<PasskeyCredential> CreateAsync(int userId, byte[] credentialId, byte[] publicKey, long signCount, Guid aaGuid);
    Task UpdateSignCountAsync(int id, long signCount);
    Task<PasskeyCredential?> UpdateNameAsync(int id, int userId, string? name);
    Task DeleteByIdAsync(int id, int userId);
}