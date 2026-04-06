-- Returns all exchange rates for the specified closing date.
SELECT
    er.Date,
    er.Currency,
    er.ExchangeRate AS Rate,
    er.InsertedAtUtc
FROM ExchangeRates er
WHERE er.Date = @Date
ORDER BY er.Currency;
