SELECT Id,
       SettlementId,
       Name,
       Value,
       Currency,
       AddedBy,
       AddedAtUtc,
       DistributionMode
FROM   Entries
WHERE  Id = @Id;