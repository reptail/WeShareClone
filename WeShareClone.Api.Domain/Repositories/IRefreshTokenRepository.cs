using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(int userId, string token, DateTime expiresAtUtc);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task DeleteByTokenAsync(string token);
}