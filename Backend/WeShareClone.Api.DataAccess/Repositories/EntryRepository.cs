using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.Api.DataAccess.Extensions;
using WeShareClone.Api.DataAccess.Models;
using WeShareClone.Api.DataAccess.Sql;
using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Domain.Repositories;

namespace WeShareClone.Api.DataAccess.Repositories;

public class EntryRepository(Func<SqlConnection> connectionFactory) : IEntryRepository
{
    public async Task<Entry[]> GetBySettlementIdAsync(int settlementId)
    {
        using SqlConnection connection = connectionFactory();
        IEnumerable<DbEntry> rows = await connection.QueryAsync<DbEntry>(
            sql: SqlScripts.GetEntriesBySettlementId,
            param: new { SettlementId = settlementId }
        );
        IEnumerable<DbEntryDistribution> distRows = await connection.QueryAsync<DbEntryDistribution>(
            sql: SqlScripts.GetEntryDistributionsBySettlementId,
            param: new { SettlementId = settlementId }
        );

        Dictionary<int, IReadOnlyList<EntryDistribution>> distsByEntryId = distRows
            .GroupBy(d => d.EntryId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<EntryDistribution>)g.Select(d => d.ToDomain()).ToArray()
            );

        return rows
            .Select(row => row.ToDomain(distsByEntryId.GetValueOrDefault(row.Id, [])))
            .ToArray();
    }

    public async Task<Entry?> GetByIdAsync(int id)
    {
        using SqlConnection connection = connectionFactory();
        DbEntry? row = await connection.QueryFirstOrDefaultAsync<DbEntry>(
            sql: SqlScripts.GetEntryById,
            param: new { Id = id }
        );
        if (row is null)
            return null;

        IEnumerable<DbEntryDistribution> distRows = await connection.QueryAsync<DbEntryDistribution>(
            sql: SqlScripts.GetEntryDistributionsByEntryId,
            param: new { EntryId = id }
        );
        return row.ToDomain(distRows.Select(d => d.ToDomain()).ToArray());
    }

    public async Task<Entry> CreateAsync(Entry entry)
    {
        using SqlConnection connection = connectionFactory();
        await connection.OpenAsync();
        using SqlTransaction transaction = connection.BeginTransaction();

        DbEntry row = await connection.QuerySingleAsync<DbEntry>(
            sql: SqlScripts.CreateEntry,
            param: new
            {
                entry.SettlementId,
                entry.Name,
                entry.Value,
                entry.Currency,
                entry.AddedBy,
            },
            transaction: transaction
        );

        foreach (EntryDistribution dist in entry.Distributions)
        {
            await connection.ExecuteAsync(
                sql: SqlScripts.CreateEntryDistribution,
                param: new { EntryId = row.Id, dist.UserId, dist.Factor },
                transaction: transaction
            );
        }

        await transaction.CommitAsync();
        return row.ToDomain(entry.Distributions);
    }

    public async Task<Entry?> UpdateAsync(Entry entry)
    {
        using SqlConnection connection = connectionFactory();
        await connection.OpenAsync();
        using SqlTransaction transaction = connection.BeginTransaction();

        DbEntry? row = await connection.QuerySingleOrDefaultAsync<DbEntry>(
            sql: SqlScripts.UpdateEntry,
            param: new
            {
                entry.Id,
                entry.Name,
                entry.Value,
                entry.Currency,
            },
            transaction: transaction
        );

        if (row is null)
        {
            await transaction.RollbackAsync();
            return null;
        }

        await connection.ExecuteAsync(
            sql: SqlScripts.DeleteEntryDistributionsByEntryId,
            param: new { EntryId = entry.Id },
            transaction: transaction
        );

        foreach (EntryDistribution dist in entry.Distributions)
        {
            await connection.ExecuteAsync(
                sql: SqlScripts.CreateEntryDistribution,
                param: new { EntryId = row.Id, dist.UserId, dist.Factor },
                transaction: transaction
            );
        }

        await transaction.CommitAsync();
        return row.ToDomain(entry.Distributions);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using SqlConnection connection = connectionFactory();
        int affected = await connection.ExecuteAsync(
            sql: SqlScripts.DeleteEntry,
            param: new { Id = id }
        );
        return affected > 0;
    }
}
