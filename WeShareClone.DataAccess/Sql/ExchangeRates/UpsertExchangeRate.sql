-- Inserts a new exchange rate or updates the rate if a record already exists
-- for the same closing date and currency.
MERGE INTO ExchangeRates AS target
USING (
    SELECT
        @Date       AS Date,
        @Currency   AS Currency,
        @Rate       AS ExchangeRate
) AS source
ON  target.Date     = source.Date
AND target.Currency = source.Currency
WHEN MATCHED THEN
    UPDATE SET
        target.ExchangeRate  = source.ExchangeRate
WHEN NOT MATCHED THEN
    INSERT (Date, Currency, ExchangeRate)
    VALUES (source.Date, source.Currency, source.ExchangeRate);
