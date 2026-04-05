-- Upserts the full set of distributions for a single entry using a TVP.
-- Rows present in the target but absent from @Distributions are deleted,
-- ensuring the stored set always matches the incoming data exactly.
MERGE INTO EntryDistributions AS target
USING @Distributions AS source
    ON target.EntryId = @EntryId
   AND target.UserId  = source.UserId
WHEN MATCHED THEN
    UPDATE SET Factor = source.Factor
WHEN NOT MATCHED BY TARGET THEN
    INSERT (EntryId, UserId, Factor)
    VALUES (@EntryId, source.UserId, source.Factor)
WHEN NOT MATCHED BY SOURCE
 AND target.EntryId = @EntryId THEN
    DELETE;
