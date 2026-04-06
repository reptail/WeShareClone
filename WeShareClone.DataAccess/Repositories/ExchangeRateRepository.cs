using Dapper;
using Microsoft.Data.SqlClient;
using WeShareClone.DataAccess.Extensions;
using WeShareClone.DataAccess.Models;
using WeShareClone.DataAccess.Sql;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.DataAccess.Repositories;

public class ExchangeRateRepository(Func<SqlConnection> connectionFactory) : IExchangeRateRepository
{
    public async Task<ExchangeRate[]> GetLatestAsync()
    {
        using SqlConnection connection = connectionFactory();
        IEnumerable<DbExchangeRate> rows = await connection.QueryAsync<DbExchangeRate>(
            SqlScripts.GetLatestExchangeRates
        );
        return rows.Select(r => r.ToDomain()).ToArray();
    }

    public async Task<ExchangeRate[]> GetByDateAsync(DateOnly date)
    {
        using SqlConnection connection = connectionFactory();
        IEnumerable<DbExchangeRate> rows = await connection.QueryAsync<DbExchangeRate>(
            SqlScripts.GetExchangeRatesByDate,
            new { Date = date }
        );
        return rows.Select(r => r.ToDomain()).ToArray();
    }

    public async Task UpsertManyAsync(IEnumerable<ExchangeRate> rates)
    {
        using SqlConnection connection = connectionFactory();
        foreach (ExchangeRate rate in rates)
        {
            await connection.ExecuteAsync(
                SqlScripts.UpsertExchangeRate,
                new
                {
                    Date     = rate.Date,
                    Currency = rate.Currency,
                    Rate     = rate.Rate,
                }
            );
        }
    }
}
