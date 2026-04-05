-- ============================================================
-- 002_AddEntryDistributionTVP.sql
-- Creates a User-Defined Table Type used to pass a batch of
-- entry distributions to the UpsertEntryDistributions stored
-- procedure / MERGE statement via a Table-Valued Parameter.
-- ============================================================

USE WeShareClone;
GO

CREATE TYPE dbo.EntryDistributionTableType AS TABLE (
    UserId INT           NOT NULL,
    Factor DECIMAL(18,6) NOT NULL
);
GO
