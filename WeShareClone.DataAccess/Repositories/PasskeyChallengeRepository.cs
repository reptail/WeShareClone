using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

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