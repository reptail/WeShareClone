-- ============================================================
-- 007_SettlementClosing.sql
-- Replaces the IsOpen BIT column on Settlements with a
-- Status TINYINT (0=Open, 1=BeingSettled, 2=Closed) to support
-- a three-state settlement lifecycle.
-- Adds the SettlementDebt table to persist calculated debts
-- and track individual payment status.
-- ============================================================

USE WeShareClone;
GO

-- ============================================================
-- Settlements: replace IsOpen with Status
-- ============================================================
ALTER TABLE Settlements
    ADD Status TINYINT NOT NULL CONSTRAINT DF_Settlements_Status DEFAULT 0;
GO

-- Migrate existing data: IsOpen=1 (open) → Status=0, IsOpen=0 (closed) → Status=2
UPDATE Settlements
SET    Status = CASE WHEN IsOpen = 1 THEN 0 ELSE 2 END;
GO

ALTER TABLE Settlements
    DROP CONSTRAINT DF_Settlements_IsOpen;
GO

ALTER TABLE Settlements
    DROP COLUMN IsOpen;
GO

-- ============================================================
-- SettlementDebt
-- Persisted debts calculated when a settlement enters the
-- BeingSettled state. Deleted when settlement reverts to Open.
-- ============================================================
CREATE TABLE SettlementDebt (
    Id           INT            NOT NULL IDENTITY(1,1),
    SettlementId INT            NOT NULL,
    FromUserId   INT            NOT NULL,
    ToUserId     INT            NOT NULL,
    Amount       DECIMAL(18,2)  NOT NULL,
    Currency     NCHAR(3)       NOT NULL,
    IsPaid       BIT            NOT NULL CONSTRAINT DF_SettlementDebt_IsPaid      DEFAULT 0,
    PaidAtUtc    DATETIME2(3)       NULL,
    CreatedAtUtc DATETIME2(3)   NOT NULL CONSTRAINT DF_SettlementDebt_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2(3)   NOT NULL CONSTRAINT DF_SettlementDebt_UpdatedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_SettlementDebt                 PRIMARY KEY (Id),
    CONSTRAINT FK_SettlementDebt_SettlementId    FOREIGN KEY (SettlementId) REFERENCES Settlements (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_SettlementDebt_FromUserId      FOREIGN KEY (FromUserId)   REFERENCES Users (Id)       ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_SettlementDebt_ToUserId        FOREIGN KEY (ToUserId)     REFERENCES Users (Id)       ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO
