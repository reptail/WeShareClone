SELECT Id,
       SettlementId,
       Name,
       Value,
       Currency,
       AddedBy,
       AddedAtUtc
FROM   Entries
WHERE  SettlementId = @SettlementId
ORDER BY AddedAtUtc;