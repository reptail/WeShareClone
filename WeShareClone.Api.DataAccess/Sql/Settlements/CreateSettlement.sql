INSERT INTO Settlements (Name, Thumbnail, Currency, CreatedBy)
OUTPUT INSERTED.Id,
       INSERTED.Name,
       INSERTED.Thumbnail,
       INSERTED.Currency,
       INSERTED.CreatedBy,
       INSERTED.CreatedAtUtc
VALUES (@Name, @Thumbnail, @Currency, @CreatedBy);