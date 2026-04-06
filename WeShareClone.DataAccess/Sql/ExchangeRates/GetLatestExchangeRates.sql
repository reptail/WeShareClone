-- Returns the current revision of each exchange rate (ValidToUtc = sentinel max).
-- @currencies: optional filter; if empty, returns all currencies.
SELECT
    er.Currency,
    er.ExchangeRate AS Rate,
    er.Date,
    er.ValidFromUtc,
    er.ValidToUtc
FROM ExchangeRates er
WHERE er.ValidToUtc = '9999-12-31 23:59:59.999'
  AND (NOT EXISTS(SELECT 1 FROM @currencies) OR er.Currency IN (SELECT cv.Value FROM @currencies cv))
ORDER BY er.Currency;
