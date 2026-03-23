using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.Api.DataAccess.Extensions;
using WeShareClone.Api.DataAccess.Models;
using WeShareClone.Api.DataAccess.Sql;
using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Domain.Repositories;

namespace WeShareClone.Api.DataAccess.Repositories;

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