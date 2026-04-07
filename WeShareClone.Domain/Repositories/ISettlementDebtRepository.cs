using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface ISettlementDebtRepository
{
    Task<SettlementDebt[]> GetBySettlementIdAsync(int settlementId);
    Task<SettlementDebt> CreateAsync(SettlementDebt debt);
    Task<SettlementDebt?> UpdatePaidAsync(int debtId, bool isPaid);
    Task DeleteBySettlementIdAsync(int settlementId);
}
