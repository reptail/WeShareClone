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
        return rows.Select(row => row.ToDomain()).ToArray();
    }

    public async Task<Entry?> GetByIdAsync(int id)
    {
        using SqlConnection connection = connectionFactory();
        DbEntry? row = await connection.QueryFirstOrDefaultAsync<DbEntry>(
            sql: SqlScripts.GetEntryById,
            param: new { Id = id }
        );
        return row?.ToDomain();
    }

    public async Task<Entry> CreateAsync(Entry entry)
    {
        using SqlConnection connection = connectionFactory();
        DbEntry row = await connection.QuerySingleAsync<DbEntry>(
            sql: SqlScripts.CreateEntry,
            param: new
            {
                entry.SettlementId,
                entry.Name,
                entry.Value,
                entry.Currency,
                entry.AddedBy,
            }
        );
        return row.ToDomain();
    }

    public async Task<Entry?> UpdateAsync(Entry entry)
    {
        using SqlConnection connection = connectionFactory();
        DbEntry? row = await connection.QuerySingleOrDefaultAsync<DbEntry>(
            sql: SqlScripts.UpdateEntry,
            param: new
            {
                entry.Id,
                entry.Name,
                entry.Value,
                entry.Currency,
            }
        );
        return row?.ToDomain();
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