UPDATE Settlements
SET    Name      = @Name,
       Thumbnail = @Thumbnail,
       Currency  = @Currency,
       IsOpen    = @IsOpen
OUTPUT INSERTED.Id,
       INSERTED.Name,
       INSERTED.Thumbnail,
       INSERTED.Currency,
       INSERTED.CreatedBy,
       INSERTED.CreatedAtUtc,
       INSERTED.IsOpen
WHERE  Id = @Id;