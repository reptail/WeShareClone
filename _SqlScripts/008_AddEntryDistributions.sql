-- ============================================================
-- 008_AddEntryDistributions.sql
-- Adds the EntryDistributions table for per-user distribution
-- factors on entries
-- ============================================================

USE WeShareClone;
GO

CREATE TABLE EntryDistributions (
    EntryId  INT            NOT NULL,
    UserId   INT            NOT NULL,
    Factor   DECIMAL(18,6)  NOT NULL,

    CONSTRAINT PK_EntryDistributions          PRIMARY KEY (EntryId, UserId),
    CONSTRAINT FK_EntryDistributions_EntryId  FOREIGN KEY (EntryId) REFERENCES Entries (Id) ON DELETE CASCADE,
    CONSTRAINT FK_EntryDistributions_UserId   FOREIGN KEY (UserId)  REFERENCES Users   (Id)
);
GO
