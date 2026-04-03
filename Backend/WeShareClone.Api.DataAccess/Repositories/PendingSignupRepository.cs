using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.Api.DataAccess.Extensions;
using WeShareClone.Api.DataAccess.Models;
using WeShareClone.Api.DataAccess.Sql;
using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Domain.Repositories;

namespace WeShareClone.Api.DataAccess.Repositories;

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

    public async Task UpsertAsync(string email, string name)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.UpsertPendingSignup,
            param: new { Email = email, Name = name }
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