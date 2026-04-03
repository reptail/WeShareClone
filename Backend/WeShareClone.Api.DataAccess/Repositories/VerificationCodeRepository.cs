using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.Api.DataAccess.Extensions;
using WeShareClone.Api.DataAccess.Models;
using WeShareClone.Api.DataAccess.Sql;
using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Domain.Repositories;

namespace WeShareClone.Api.DataAccess.Repositories;

public class VerificationCodeRepository(Func<SqlConnection> connectionFactory) : IVerificationCodeRepository
{
    public async Task<VerificationCode?> GetByEmailAsync(string email)
    {
        using SqlConnection connection = connectionFactory();
        DbVerificationCode? row = await connection.QueryFirstOrDefaultAsync<DbVerificationCode>(
            sql: SqlScripts.GetVerificationCodeByEmail,
            param: new { Email = email }
        );
        return row?.ToDomain();
    }

    public async Task UpsertAsync(string email, string codeHash, DateTime expiresAtUtc)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.UpsertVerificationCode,
            param: new { Email = email, CodeHash = codeHash, ExpiresAtUtc = expiresAtUtc }
        );
    }

    public async Task DeleteByEmailAsync(string email)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.DeleteVerificationCodeByEmail,
            param: new { Email = email }
        );
    }
}