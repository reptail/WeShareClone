using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.Api.DataAccess.Extensions;
using WeShareClone.Api.DataAccess.Models;
using WeShareClone.Api.DataAccess.Sql;
using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Domain.Repositories;

namespace WeShareClone.Api.DataAccess.Repositories;

public class SettlementRepository(Func<SqlConnection> connectionFactory) : ISettlementRepository
{
    public async Task<bool> IsParticipantAsync(int settlementId, int userId)
    {
        using SqlConnection connection = connectionFactory();
        int result = await connection.ExecuteScalarAsync<int>(
            sql: SqlScripts.IsSettlementParticipant,
            param: new { SettlementId = settlementId, UserId = userId }
        );
        return result == 1;
    }

    public async Task<Settlement[]> GetAllAsync()
    {
        using SqlConnection connection = connectionFactory();
        IEnumerable<DbSettlement> rows = await connection.QueryAsync<DbSettlement>(
            sql: SqlScripts.GetAllSettlements
        );
        return rows.Select(row => row.ToDomain()).ToArray();
    }

    public async Task<Settlement?> GetByIdAsync(int id)
    {
        using SqlConnection connection = connectionFactory();
        DbSettlement? row = await connection.QueryFirstOrDefaultAsync<DbSettlement>(
            sql: SqlScripts.GetSettlementById,
            param: new { Id = id }
        );
        return row?.ToDomain();
    }

    public async Task<Settlement> CreateAsync(Settlement settlement)
    {
        using SqlConnection connection = connectionFactory();
        DbSettlement row = await connection.QuerySingleAsync<DbSettlement>(
            sql: SqlScripts.CreateSettlement,
            param: new
            {
                settlement.Name,
                settlement.Thumbnail,
                settlement.Currency,
                settlement.CreatedBy,
            }
        );
        return row.ToDomain();
    }

    public async Task<Settlement?> UpdateAsync(Settlement settlement)
    {
        using SqlConnection connection = connectionFactory();
        DbSettlement? row = await connection.QuerySingleOrDefaultAsync<DbSettlement>(
            sql: SqlScripts.UpdateSettlement,
            param: new
            {
                settlement.Id,
                settlement.Name,
                settlement.Thumbnail,
                settlement.Currency,
            }
        );
        return row?.ToDomain();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using SqlConnection connection = connectionFactory();
        int affected = await connection.ExecuteAsync(
            sql: SqlScripts.DeleteSettlement,
            param: new { Id = id }
        );
        return affected > 0;
    }

    public async Task AddUserAsync(int settlementId, int userId)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.AddUserToSettlement,
            param: new { SettlementId = settlementId, UserId = userId }
        );
    }

    public async Task RemoveUserAsync(int settlementId, int userId)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.RemoveUserFromSettlement,
            param: new { SettlementId = settlementId, UserId = userId }
        );
    }
}