INSERT INTO Settlements (Name, Thumbnail, Currency, CreatedBy, IsOpen)
OUTPUT INSERTED.Id,
       INSERTED.Name,
       INSERTED.Thumbnail,
       INSERTED.Currency,
       INSERTED.CreatedBy,
       INSERTED.CreatedAtUtc,
       INSERTED.IsOpen
VALUES (@Name, @Thumbnail, @Currency, @CreatedBy, @IsOpen);