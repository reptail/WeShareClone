SELECT Id,
       Name,
       Thumbnail,
       Currency,
       CreatedBy,
       CreatedAtUtc,
       IsOpen
FROM   Settlements
WHERE  Id = @Id;