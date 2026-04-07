-- ============================================================
-- 005_AddPhoneNumber.sql
-- Adds nullable Phone column to Users and PendingSignups tables.
-- ============================================================

USE WeShareClone;
GO

ALTER TABLE Users
    ADD Phone NVARCHAR(50) NULL;
GO

ALTER TABLE PendingSignups
    ADD Phone NVARCHAR(50) NULL;
GO
