-- ============================================================
-- 001_InitialSchema.sql
-- Creates the WeShareClone database and initial table structure
-- ============================================================

USE master;
GO

CREATE DATABASE WeShareClone
    COLLATE Danish_Norwegian_CI_AS;
GO

USE WeShareClone;
GO

-- ============================================================
-- Users
-- ============================================================
CREATE TABLE Users (
    Id            INT               NOT NULL IDENTITY(1,1),
    Email         NVARCHAR(256)     NOT NULL,
    Name          NVARCHAR(256)     NOT NULL,
    JoinedAtUtc   DATETIME2(3)      NOT NULL CONSTRAINT DF_Users_JoinedAtUtc    DEFAULT SYSUTCDATETIME(),
    IsDeleted     BIT               NOT NULL CONSTRAINT DF_Users_IsDeleted      DEFAULT 0,

    CONSTRAINT PK_Users       PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Email UNIQUE      (Email)
);
GO

-- ============================================================
-- Settlements
-- ============================================================
CREATE TABLE Settlements (
    Id            INT               NOT NULL IDENTITY(1,1),
    Name          NVARCHAR(256)     NOT NULL,
    Thumbnail     NVARCHAR(2048)        NULL,   -- URL to settlement image
    Currency      NCHAR(3)          NOT NULL,   -- ISO 4217 currency code (e.g. DKK, EUR, USD)
    CreatedBy     INT               NOT NULL,
    CreatedAtUtc  DATETIME2(3)      NOT NULL CONSTRAINT DF_Settlements_CreatedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Settlements           PRIMARY KEY (Id),
    CONSTRAINT FK_Settlements_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users (Id)
);
GO

-- ============================================================
-- Entries
-- ============================================================
CREATE TABLE Entries (
    Id            INT               NOT NULL IDENTITY(1,1),
    SettlementId  INT               NOT NULL,
    Name          NVARCHAR(256)     NOT NULL,
    Value         DECIMAL(18, 2)    NOT NULL,
    Currency      NCHAR(3)          NOT NULL,   -- ISO 4217 currency code
    AddedBy       INT               NOT NULL,
    AddedAtUtc    DATETIME2(3)      NOT NULL CONSTRAINT DF_Entries_AddedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Entries               PRIMARY KEY (Id),
    CONSTRAINT FK_Entries_SettlementId  FOREIGN KEY (SettlementId) REFERENCES Settlements (Id),
    CONSTRAINT FK_Entries_AddedBy       FOREIGN KEY (AddedBy)      REFERENCES Users (Id)
);
GO

-- ============================================================
-- Exchange Rate cache
-- ============================================================
CREATE TABLE ExchangeRates (
    Date            DATE            NOT NULL,
    Currency        NCHAR(3)        NOT NULL,   -- ISO 4217 currency code (target currency; base is always DKK)
    ExchangeRate    DECIMAL(18, 6)  NOT NULL,   -- Cost in DKK for 100 units of Currency
    InsertedAtUtc   DATETIME2(3)    NOT NULL CONSTRAINT DF_ExchangeRates_InsertedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_ExchangeRates PRIMARY KEY (Date, Currency)
);
GO
