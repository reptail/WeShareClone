-- ============================================================
-- 003_AddSettlementUsers.sql
-- Adds a junction table linking users to settlements
-- ============================================================

USE WeShareClone;
GO

CREATE TABLE SettlementUsers (
    SettlementId  INT          NOT NULL,
    UserId        INT          NOT NULL,
    JoinedAtUtc   DATETIME2(3) NOT NULL CONSTRAINT DF_SettlementUsers_JoinedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_SettlementUsers             PRIMARY KEY (SettlementId, UserId),
    CONSTRAINT FK_SettlementUsers_Settlement  FOREIGN KEY (SettlementId) REFERENCES Settlements (Id),
    CONSTRAINT FK_SettlementUsers_User        FOREIGN KEY (UserId)       REFERENCES Users (Id)
);
GO