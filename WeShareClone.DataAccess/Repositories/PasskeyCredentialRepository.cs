using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

public class PasskeyCredentialRepository(Func<SqlConnection> connectionFactory) : IPasskeyCredentialRepository
{
    public async Task<PasskeyCredential[]> GetByUserIdAsync(int userId)
    {
        using SqlConnection connection = connectionFactory();
        IEnumerable<DbPasskeyCredential> rows = await connection.QueryAsync<DbPasskeyCredential>(
            sql: SqlScripts.GetPasskeyCredentialsByUserId,
            param: new { UserId = userId }
        );
        return rows.Select(static r => r.ToDomain()).ToArray();
    }

    public async Task<PasskeyCredential?> GetByCredentialIdAsync(byte[] credentialId)
    {
        using SqlConnection connection = connectionFactory();
        DbPasskeyCredential? row = await connection.QueryFirstOrDefaultAsync<DbPasskeyCredential>(
            sql: SqlScripts.GetPasskeyCredentialByCredentialId,
            param: new { CredentialId = credentialId }
        );
        return row?.ToDomain();
    }

    public async Task<PasskeyCredential> CreateAsync(int userId, byte[] credentialId, byte[] publicKey, long signCount, Guid aaGuid)
    {
        using SqlConnection connection = connectionFactory();
        DbPasskeyCredential row = await connection.QuerySingleAsync<DbPasskeyCredential>(
            sql: SqlScripts.CreatePasskeyCredential,
            param: new { UserId = userId, CredentialId = credentialId, PublicKey = publicKey, SignCount = signCount, AaGuid = aaGuid }
        );
        return row.ToDomain();
    }

    public async Task UpdateSignCountAsync(int id, long signCount)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.UpdatePasskeySignCount,
            param: new { Id = id, SignCount = signCount }
        );
    }

    public async Task<PasskeyCredential?> UpdateNameAsync(int id, int userId, string? name)
    {
        using SqlConnection connection = connectionFactory();
        DbPasskeyCredential? row = await connection.QueryFirstOrDefaultAsync<DbPasskeyCredential>(
            sql: SqlScripts.UpdatePasskeyName,
            param: new { Id = id, UserId = userId, Name = name }
        );
        return row?.ToDomain();
    }

    public async Task DeleteByIdAsync(int id, int userId)
    {
        using SqlConnection connection = connectionFactory();
        await connection.ExecuteAsync(
            sql: SqlScripts.DeletePasskeyCredentialById,
            param: new { Id = id, UserId = userId }
        );
    }
}