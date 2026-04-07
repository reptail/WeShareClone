SELECT Id,
       Name,
       Thumbnail,
       Currency,
       CreatedBy,
       CreatedAtUtc,
       Status
FROM   Settlements
WHERE  Id = @Id;