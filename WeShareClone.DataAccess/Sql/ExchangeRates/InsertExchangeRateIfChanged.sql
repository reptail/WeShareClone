-- Inserts a new revision for the given currency only when the rate has changed.
--
-- Logic:
--   1. If a current revision (ValidToUtc = sentinel max) exists with the same
--      ExchangeRate value → no-op (rate unchanged).
--   2. Otherwise:
--      a. Close the existing current revision by setting ValidToUtc = @ValidFromUtc.
--      b. Insert a new revision with ValidToUtc = sentinel max.

DECLARE @MaxDate DATETIME2(3) = '9999-12-31 23:59:59.999';

IF NOT EXISTS (
    SELECT 1
    FROM   ExchangeRates
    WHERE  Currency     = @Currency
      AND  ExchangeRate = @Rate
      AND  ValidToUtc   = @MaxDate
)
BEGIN
    -- Close the current revision (if one exists).
    UPDATE ExchangeRates
    SET    ValidToUtc = @ValidFromUtc
    WHERE  Currency   = @Currency
      AND  ValidToUtc = @MaxDate;

    -- Insert the new current revision.
    INSERT INTO ExchangeRates (Currency, ExchangeRate, Date, ValidFromUtc, ValidToUtc)
    VALUES (@Currency, @Rate, @Date, @ValidFromUtc, @MaxDate);
END
