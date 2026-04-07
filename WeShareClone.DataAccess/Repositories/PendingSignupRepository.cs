using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

public class PendingSignupRepository(Func<SqlConnection> connectionFactory) : IPendingSignupRepository
{
    public async Task<PendingSignup?> GetByEmailAsync(string email)
    {
        using SqlConnection connection = connectionFactory();
        DbPendingSignup? row = await connection.QueryFirstOrDefaultAsync<DbPendingSignup>(
            sql: SqlScripts.GetPendingSignupByEmail,
            param: new { Email = email }
        );
        return row?.ToDomain();
    }

    public async Task UpsertAsync(string email, string name, string? phone = null)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.UpsertPendingSignup,
            param: new { Email = email, Name = name, Phone = phone }
        );
    }

    public async Task DeleteByEmailAsync(string email)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.DeletePendingSignupByEmail,
            param: new { Email = email }
        );
    }
}