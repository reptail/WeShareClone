using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

public class UserRepository(Func<SqlConnection> connectionFactory) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        using SqlConnection connection = connectionFactory();
        DbUser? row = await connection.QueryFirstOrDefaultAsync<DbUser>(
            sql: SqlScripts.GetUserByEmail,
            param: new { Email = email }
        );
        return row?.ToDomain();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using SqlConnection connection = connectionFactory();
        DbUser? row = await connection.QueryFirstOrDefaultAsync<DbUser>(
            sql: SqlScripts.GetUserById,
            param: new { Id = id }
        );
        return row?.ToDomain();
    }

    public async Task<User> CreateAsync(string email, string name)
    {
        using SqlConnection connection = connectionFactory();
        DbUser row = await connection.QuerySingleAsync<DbUser>(
            sql: SqlScripts.CreateUser,
            param: new { Email = email, Name = name }
        );
        return row.ToDomain();
    }

    public async Task<User?> UpdateNameAsync(int id, string name)
    {
        using SqlConnection connection = connectionFactory();
        DbUser? row = await connection.QuerySingleOrDefaultAsync<DbUser>(
            sql: SqlScripts.UpdateUserName,
            param: new { Id = id, Name = name }
        );
        return row?.ToDomain();
    }
}