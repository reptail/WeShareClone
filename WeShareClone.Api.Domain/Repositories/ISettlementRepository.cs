using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.Domain.Repositories;

public interface ISettlementRepository
{
    Task<Settlement[]> GetAllAsync();
    Task<Settlement?> GetByIdAsync(int id);
    Task<Settlement> CreateAsync(Settlement settlement);
    Task<Settlement?> UpdateAsync(Settlement settlement);
    Task<bool> DeleteAsync(int id);

    Task AddUserAsync(int settlementId, int userId);
    Task RemoveUserAsync(int settlementId, int userId);
}