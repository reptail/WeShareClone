using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.Domain.Repositories;

public interface IEntryRepository
{
    Task<Entry[]> GetBySettlementIdAsync(int settlementId);
    Task<Entry?> GetByIdAsync(int id);
    Task<Entry> CreateAsync(Entry entry);
    Task<Entry> UpdateAsync(Entry entry);
    Task DeleteAsync(int id);
}