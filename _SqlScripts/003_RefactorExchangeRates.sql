-- ============================================================
-- Refactor ExchangeRates to append-only revision history.
--
-- Changes from 001_CreateSchema.sql:
--   - PK changed from (Date, Currency) to (ValidToUtc, Currency)
--   - InsertedAtUtc replaced by ValidFromUtc / ValidToUtc (SCD Type 2)
--   - ValidToUtc = '9999-12-31 23:59:59.999' identifies the current revision
--   - Date column retained as informational (closing date from source)
-- ============================================================

DROP TABLE ExchangeRates;
GO

CREATE TABLE ExchangeRates (
    Currency       NCHAR(3)        NOT NULL,   -- ISO 4217 currency code (target; base is always DKK)
    ExchangeRate   DECIMAL(18, 6)  NOT NULL,   -- Cost in DKK for 100 units of Currency
    Date           DATE            NOT NULL,   -- Closing date from source (informational)
    ValidFromUtc   DATETIME2(3)    NOT NULL,
    ValidToUtc     DATETIME2(3)    NOT NULL,   -- '9999-12-31 23:59:59.999' = current revision

    CONSTRAINT PK_ExchangeRates PRIMARY KEY (ValidToUtc, Currency)
);
GO
