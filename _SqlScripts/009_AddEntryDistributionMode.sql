-- ============================================================
-- 009_AddEntryDistributionMode.sql
-- Adds the DistributionMode column to the Entries table to
-- record how the cost was split (even, percentage, fixed)
-- ============================================================
-- NOTE: Existing entries predate this column and cannot have
-- their mode retroactively determined from factors alone
-- (e.g. equal factors are valid for both EvenSplit and
-- Percentage). All existing rows default to EvenSplit (0)
-- and should be reviewed and updated manually if required.
-- ============================================================

USE WeShareClone;
GO

ALTER TABLE Entries
    ADD DistributionMode TINYINT NOT NULL
        CONSTRAINT DF_Entries_DistributionMode DEFAULT 0;
GO
