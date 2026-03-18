SELECT Id,
       SettlementId,
       Name,
       Value,
       Currency,
       AddedBy,
       AddedAtUtc
FROM   Entries
WHERE  Id = @Id;