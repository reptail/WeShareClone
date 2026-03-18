SELECT Id,
       Name,
       Thumbnail,
       Currency,
       CreatedBy,
       CreatedAtUtc
FROM   Settlements
WHERE  Id = @Id;