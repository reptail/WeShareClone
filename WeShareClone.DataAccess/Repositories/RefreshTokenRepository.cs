using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

public class RefreshTokenRepository(Func<SqlConnection> connectionFactory) : IRefreshTokenRepository
{
    public async Task<RefreshToken> CreateAsync(int userId, string token, DateTime expiresAtUtc)
    {
        using SqlConnection connection = connectionFactory();
        DbRefreshToken row = await connection.QuerySingleAsync<DbRefreshToken>(
            sql: SqlScripts.CreateRefreshToken,
            param: new { UserId = userId, Token = token, ExpiresAtUtc = expiresAtUtc }
        );
        return row.ToDomain();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        using SqlConnection connection = connectionFactory();
        DbRefreshToken? row = await connection.QueryFirstOrDefaultAsync<DbRefreshToken>(
            sql: SqlScripts.GetRefreshTokenByToken,
            param: new { Token = token }
        );
        return row?.ToDomain();
    }

    public async Task DeleteByTokenAsync(string token)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.DeleteRefreshTokenByToken,
            param: new { Token = token }
        );
    }
}