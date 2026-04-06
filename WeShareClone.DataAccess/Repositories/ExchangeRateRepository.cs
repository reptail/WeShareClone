using System.Data;
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
    public async Task<ExchangeRate[]> GetLatestAsync(IEnumerable<string>? currencies = null)
    {
        DataTable currenciesTvp = BuildStringValuesTvp(currencies);

        using SqlConnection connection = connectionFactory();
        IEnumerable<DbExchangeRate> rows = await connection.QueryAsync<DbExchangeRate>(
            SqlScripts.GetLatestExchangeRates,
            new { currencies = currenciesTvp.AsTableValuedParameter("dbo.StringValues") }
        );
        return rows.Select(r => r.ToDomain()).ToArray();
    }

    public async Task InsertManyIfChangedAsync(IEnumerable<ExchangeRate> rates)
    {
        using SqlConnection connection = connectionFactory();
        foreach (ExchangeRate rate in rates)
        {
            await connection.ExecuteAsync(
                SqlScripts.InsertExchangeRateIfChanged,
                new
                {
                    Currency     = rate.Currency,
                    Rate         = rate.Rate,
                    Date         = rate.Date,
                    ValidFromUtc = rate.ValidFromUtc,
                }
            );
        }
    }

    private static DataTable BuildStringValuesTvp(IEnumerable<string>? values)
    {
        DataTable table = new();
        table.Columns.Add("Value", typeof(string));

        if (values is not null)
        {
            foreach (string value in values)
                table.Rows.Add(value);
        }

        return table;
    }
}
