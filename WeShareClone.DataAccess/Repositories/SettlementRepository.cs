using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

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

    public async Task<Settlement[]> GetByUserIdAsync(int userId)
    {
        using SqlConnection connection = connectionFactory();
        IEnumerable<DbSettlement> rows = await connection.QueryAsync<DbSettlement>(
            sql: SqlScripts.GetMySettlements,
            param: new { UserId = userId }
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
                settlement.IsOpen,
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
                settlement.IsOpen,
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

    public async Task<int[]> GetParticipantIdsAsync(int settlementId)
    {
        using SqlConnection connection = connectionFactory();
        IEnumerable<int> ids = await connection.QueryAsync<int>(
            sql: SqlScripts.GetSettlementParticipantIds,
            param: new { SettlementId = settlementId }
        );
        return ids.ToArray();
    }

    public async Task<User[]> GetParticipantsAsync(int settlementId)
    {
        using SqlConnection connection = connectionFactory();
        IEnumerable<DbUser> rows = await connection.QueryAsync<DbUser>(
            sql: SqlScripts.GetSettlementParticipants,
            param: new { SettlementId = settlementId }
        );
        return rows.Select(row => row.ToDomain()).ToArray();
    }
}
