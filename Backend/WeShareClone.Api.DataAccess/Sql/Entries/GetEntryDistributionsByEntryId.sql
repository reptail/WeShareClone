SELECT EntryId,
       UserId,
       Factor
FROM   EntryDistributions
WHERE  EntryId = @EntryId;
