using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

public class SettlementDebtRepository(Func<SqlConnection> connectionFactory) : ISettlementDebtRepository
{
    public async Task<SettlementDebt[]> GetBySettlementIdAsync(int settlementId)
    {
        using SqlConnection connection = connectionFactory();
        IEnumerable<DbSettlementDebt> rows = await connection.QueryAsync<DbSettlementDebt>(
            sql: SqlScripts.GetSettlementDebtsBySettlementId,
            param: new { SettlementId = settlementId }
        );
        return rows.Select(r => r.ToDomain()).ToArray();
    }

    public async Task<SettlementDebt> CreateAsync(SettlementDebt debt)
    {
        using SqlConnection connection = connectionFactory();
        DbSettlementDebt row = await connection.QuerySingleAsync<DbSettlementDebt>(
            sql: SqlScripts.CreateSettlementDebt,
            param: new
            {
                debt.SettlementId,
                debt.FromUserId,
                debt.ToUserId,
                debt.Amount,
                debt.Currency,
            }
        );
        return row.ToDomain();
    }

    public async Task<SettlementDebt?> UpdatePaidAsync(int debtId, bool isPaid)
    {
        using SqlConnection connection = connectionFactory();
        DbSettlementDebt? row = await connection.QuerySingleOrDefaultAsync<DbSettlementDebt>(
            sql: SqlScripts.UpdateSettlementDebtPaid,
            param: new { Id = debtId, IsPaid = isPaid }
        );
        return row?.ToDomain();
    }

    public async Task DeleteBySettlementIdAsync(int settlementId)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.DeleteSettlementDebtsBySettlementId,
            param: new { SettlementId = settlementId }
        );
    }
}
