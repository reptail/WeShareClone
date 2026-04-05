-- ============================================================
-- 001_CreateSchema.sql
-- Creates the WeShareClone database and full table structure.
-- Consolidated from scripts 001-010.
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
    Role          TINYINT           NOT NULL CONSTRAINT DF_Users_Role          DEFAULT 0,
    JoinedAtUtc   DATETIME2(3)      NOT NULL CONSTRAINT DF_Users_JoinedAtUtc   DEFAULT SYSUTCDATETIME(),
    IsDeleted     BIT               NOT NULL CONSTRAINT DF_Users_IsDeleted     DEFAULT 0,

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
    IsOpen        BIT               NOT NULL CONSTRAINT DF_Settlements_IsOpen      DEFAULT 1,
    CreatedBy     INT               NOT NULL,
    CreatedAtUtc  DATETIME2(3)      NOT NULL CONSTRAINT DF_Settlements_CreatedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Settlements           PRIMARY KEY (Id),
    CONSTRAINT FK_Settlements_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users (Id)
);
GO

-- ============================================================
-- SettlementUsers
-- ============================================================
CREATE TABLE SettlementUsers (
    SettlementId  INT          NOT NULL,
    UserId        INT          NOT NULL,
    JoinedAtUtc   DATETIME2(3) NOT NULL CONSTRAINT DF_SettlementUsers_JoinedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_SettlementUsers            PRIMARY KEY (SettlementId, UserId),
    CONSTRAINT FK_SettlementUsers_Settlement FOREIGN KEY (SettlementId) REFERENCES Settlements (Id),
    CONSTRAINT FK_SettlementUsers_User       FOREIGN KEY (UserId)       REFERENCES Users (Id)
);
GO

-- ============================================================
-- Entries
-- ============================================================
CREATE TABLE Entries (
    Id               INT               NOT NULL IDENTITY(1,1),
    SettlementId     INT               NOT NULL,
    Name             NVARCHAR(256)     NOT NULL,
    Value            DECIMAL(18, 2)    NOT NULL,
    Currency         NCHAR(3)          NOT NULL,   -- ISO 4217 currency code
    DistributionMode TINYINT           NOT NULL CONSTRAINT DF_Entries_DistributionMode DEFAULT 0,
    AddedBy          INT               NOT NULL,
    AddedAtUtc       DATETIME2(3)      NOT NULL CONSTRAINT DF_Entries_AddedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Entries              PRIMARY KEY (Id),
    CONSTRAINT FK_Entries_SettlementId FOREIGN KEY (SettlementId) REFERENCES Settlements (Id),
    CONSTRAINT FK_Entries_AddedBy      FOREIGN KEY (AddedBy)      REFERENCES Users (Id)
);
GO

-- ============================================================
-- EntryDistributions
-- ============================================================
CREATE TABLE EntryDistributions (
    EntryId  INT           NOT NULL,
    UserId   INT           NOT NULL,
    Factor   DECIMAL(18,6) NOT NULL,

    CONSTRAINT PK_EntryDistributions         PRIMARY KEY (EntryId, UserId),
    CONSTRAINT FK_EntryDistributions_EntryId FOREIGN KEY (EntryId) REFERENCES Entries (Id) ON DELETE CASCADE,
    CONSTRAINT FK_EntryDistributions_UserId  FOREIGN KEY (UserId)  REFERENCES Users   (Id)
);
GO

-- ============================================================
-- ExchangeRates
-- ============================================================
CREATE TABLE ExchangeRates (
    Date            DATE            NOT NULL,
    Currency        NCHAR(3)        NOT NULL,   -- ISO 4217 currency code (target; base is always DKK)
    ExchangeRate    DECIMAL(18, 6)  NOT NULL,   -- Cost in DKK for 100 units of Currency
    InsertedAtUtc   DATETIME2(3)    NOT NULL CONSTRAINT DF_ExchangeRates_InsertedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_ExchangeRates PRIMARY KEY (Date, Currency)
);
GO

-- ============================================================
-- VerificationCodes
-- ============================================================
CREATE TABLE VerificationCodes (
    Id           INT           NOT NULL IDENTITY(1,1),
    Email        NVARCHAR(256) NOT NULL,
    CodeHash     NVARCHAR(256) NOT NULL,
    CreatedAtUtc DATETIME2(3)  NOT NULL CONSTRAINT DF_VerificationCodes_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    ExpiresAtUtc DATETIME2(3)  NOT NULL,

    CONSTRAINT PK_VerificationCodes       PRIMARY KEY (Id),
    CONSTRAINT UQ_VerificationCodes_Email UNIQUE      (Email)
);
GO

-- ============================================================
-- RefreshTokens
-- ============================================================
CREATE TABLE RefreshTokens (
    Id           INT           NOT NULL IDENTITY(1,1),
    UserId       INT           NOT NULL,
    Token        NVARCHAR(512) NOT NULL,
    CreatedAtUtc DATETIME2(3)  NOT NULL CONSTRAINT DF_RefreshTokens_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    ExpiresAtUtc DATETIME2(3)  NOT NULL,

    CONSTRAINT PK_RefreshTokens        PRIMARY KEY (Id),
    CONSTRAINT FK_RefreshTokens_UserId FOREIGN KEY (UserId) REFERENCES Users (Id),
    CONSTRAINT UQ_RefreshTokens_Token  UNIQUE      (Token)
);
GO

-- ============================================================
-- PendingSignups
-- ============================================================
CREATE TABLE PendingSignups (
    Id           INT           NOT NULL IDENTITY(1, 1),
    Email        NVARCHAR(256) NOT NULL,
    Name         NVARCHAR(256) NOT NULL,
    CreatedAtUtc DATETIME2(3)  NOT NULL CONSTRAINT DF_PendingSignups_CreatedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_PendingSignups       PRIMARY KEY (Id),
    CONSTRAINT UQ_PendingSignups_Email UNIQUE      (Email)
);
GO

-- ============================================================
-- PasskeyCredentials
-- ============================================================
CREATE TABLE PasskeyCredentials (
    Id           INT              NOT NULL IDENTITY(1, 1),
    UserId       INT              NOT NULL,
    CredentialId VARBINARY(1024)  NOT NULL,
    PublicKey    VARBINARY(MAX)   NOT NULL,
    SignCount    BIGINT           NOT NULL CONSTRAINT DF_PasskeyCredentials_SignCount    DEFAULT 0,
    AaGuid       UNIQUEIDENTIFIER NOT NULL,
    CreatedAtUtc DATETIME2(3)     NOT NULL CONSTRAINT DF_PasskeyCredentials_CreatedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_PasskeyCredentials              PRIMARY KEY (Id),
    CONSTRAINT FK_PasskeyCredentials_UserId       FOREIGN KEY (UserId) REFERENCES Users (Id),
    CONSTRAINT UQ_PasskeyCredentials_CredentialId UNIQUE      (CredentialId)
);
GO

-- ============================================================
-- PasskeyChallenges
-- ============================================================
CREATE TABLE PasskeyChallenges (
    Id            INT           NOT NULL IDENTITY(1, 1),
    Email         NVARCHAR(256) NOT NULL,
    ChallengeType TINYINT       NOT NULL,
    OptionsJson   NVARCHAR(MAX) NOT NULL,
    ExpiresAtUtc  DATETIME2(3)  NOT NULL,

    CONSTRAINT PK_PasskeyChallenges                     PRIMARY KEY (Id),
    CONSTRAINT UQ_PasskeyChallenges_Email_ChallengeType UNIQUE      (Email, ChallengeType)
);
GO
