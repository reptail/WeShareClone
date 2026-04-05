using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(int userId, string token, DateTime expiresAtUtc);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task DeleteByTokenAsync(string token);
}