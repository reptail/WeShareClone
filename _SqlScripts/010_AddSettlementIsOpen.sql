-- ============================================================
-- 010_AddSettlementIsOpen.sql
-- Adds the IsOpen column to the Settlements table to allow
-- distinguishing between active and closed/archived settlements
-- ============================================================

USE WeShareClone;
GO

ALTER TABLE Settlements
    ADD IsOpen BIT NOT NULL
        CONSTRAINT DF_Settlements_IsOpen DEFAULT 1;
GO
