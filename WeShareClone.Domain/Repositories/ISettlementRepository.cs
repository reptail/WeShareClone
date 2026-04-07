using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface ISettlementRepository
{
    Task<bool> IsParticipantAsync(int settlementId, int userId);
    Task<Settlement[]> GetAllAsync();
    Task<Settlement?> GetByIdAsync(int id);
    Task<Settlement[]> GetByUserIdAsync(int userId);
    Task<Settlement> CreateAsync(Settlement settlement);
    Task<Settlement?> UpdateAsync(Settlement settlement);
    Task<Settlement?> UpdateStatusAsync(int id, ESettlementStatus status);
    Task<bool> DeleteAsync(int id);

    Task<int[]> GetParticipantIdsAsync(int settlementId);
    Task<User[]> GetParticipantsAsync(int settlementId);
    Task AddUserAsync(int settlementId, int userId);
    Task RemoveUserAsync(int settlementId, int userId);
}
