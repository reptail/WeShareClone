SELECT Id,
       SettlementId,
       Name,
       Value,
       Currency,
       AddedBy,
       AddedAtUtc,
       DistributionMode
FROM   Entries
WHERE  SettlementId = @SettlementId
ORDER BY AddedAtUtc;