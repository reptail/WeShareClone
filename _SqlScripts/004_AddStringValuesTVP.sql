-- ============================================================
-- 004_AddStringValuesTVP.sql
-- Creates a general-purpose User-Defined Table Type for passing
-- a list of string values as a Table-Valued Parameter.
-- ============================================================

USE WeShareClone;
GO

CREATE TYPE dbo.StringValues AS TABLE (
    Value NVARCHAR(MAX) NOT NULL
);
GO
