using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

public class EntryRepository(Func<SqlConnection> connectionFactory) : IEntryRepository
{
    /// <summary>
    /// Builds a DataTable that matches dbo.EntryDistributionTableType,
    /// used to pass distributions as a TVP to the MERGE statement.
    /// </summary>
    private static DataTable BuildDistributionsTable(IEnumerable<EntryDistribution> distributions)
    {
        DataTable table = new();
        table.Columns.Add("UserId", typeof(int));
        table.Columns.Add("Factor", typeof(decimal));

        foreach (EntryDistribution dist in distributions)
            table.Rows.Add(dist.UserId, dist.Factor);

        return table;
    }

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
                entry.DistributionMode,
            },
            transaction: transaction
        );

        await connection.ExecuteAsync(
            sql: SqlScripts.UpsertEntryDistributions,
            param: new
            {
                EntryId = row.Id,
                Distributions = BuildDistributionsTable(entry.Distributions)
                    .AsTableValuedParameter("dbo.EntryDistributionTableType"),
            },
            transaction: transaction
        );

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
                entry.DistributionMode,
            },
            transaction: transaction
        );

        if (row is null)
        {
            await transaction.RollbackAsync();
            return null;
        }

        await connection.ExecuteAsync(
            sql: SqlScripts.UpsertEntryDistributions,
            param: new
            {
                EntryId = entry.Id,
                Distributions = BuildDistributionsTable(entry.Distributions)
                    .AsTableValuedParameter("dbo.EntryDistributionTableType"),
            },
            transaction: transaction
        );

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
