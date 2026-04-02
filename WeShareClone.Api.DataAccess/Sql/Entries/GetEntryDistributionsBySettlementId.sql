SELECT ed.EntryId,
       ed.UserId,
       ed.Factor
FROM   EntryDistributions ed
INNER JOIN Entries e ON e.Id = ed.EntryId
WHERE  e.SettlementId = @SettlementId;
