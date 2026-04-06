-- Returns all exchange rates for the most recent available closing date.
SELECT
    er.Date,
    er.Currency,
    er.ExchangeRate AS Rate,
    er.InsertedAtUtc
FROM ExchangeRates er
WHERE er.Date = (
    SELECT MAX(e.Date)
    FROM ExchangeRates e
)
ORDER BY er.Currency;
