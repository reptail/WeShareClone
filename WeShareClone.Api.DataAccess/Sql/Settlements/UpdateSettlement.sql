UPDATE Settlements
SET    Name      = @Name,
       Thumbnail = @Thumbnail,
       Currency  = @Currency
OUTPUT INSERTED.Id,
       INSERTED.Name,
       INSERTED.Thumbnail,
       INSERTED.Currency,
       INSERTED.CreatedBy,
       INSERTED.CreatedAtUtc
WHERE  Id = @Id;