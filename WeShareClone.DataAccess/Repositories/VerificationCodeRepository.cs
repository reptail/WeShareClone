using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

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