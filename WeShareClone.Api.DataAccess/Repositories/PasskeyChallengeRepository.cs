using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.Api.DataAccess.Extensions;
using WeShareClone.Api.DataAccess.Models;
using WeShareClone.Api.DataAccess.Sql;
using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Domain.Repositories;

namespace WeShareClone.Api.DataAccess.Repositories;

public class PasskeyChallengeRepository(Func<SqlConnection> connectionFactory) : IPasskeyChallengeRepository
{
    public async Task<PasskeyChallenge?> GetByEmailAndTypeAsync(string email, EPasskeyChallengeType type)
    {
        using SqlConnection connection = connectionFactory();
        DbPasskeyChallenge? row = await connection.QueryFirstOrDefaultAsync<DbPasskeyChallenge>(
            sql: SqlScripts.GetPasskeyChallengeByEmailAndType,
            param: new { Email = email, ChallengeType = type }
        );
        return row?.ToDomain();
    }

    public async Task UpsertAsync(string email, EPasskeyChallengeType type, string optionsJson, DateTime expiresAtUtc)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.UpsertPasskeyChallenge,
            param: new { Email = email, ChallengeType = type, OptionsJson = optionsJson, ExpiresAtUtc = expiresAtUtc }
        );
    }

    public async Task DeleteByEmailAndTypeAsync(string email, EPasskeyChallengeType type)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.DeletePasskeyChallengeByEmailAndType,
            param: new { Email = email, ChallengeType = type }
        );
    }
}