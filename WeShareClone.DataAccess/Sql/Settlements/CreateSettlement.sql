INSERT INTO Settlements (Name, Thumbnail, Currency, CreatedBy, Status)
OUTPUT INSERTED.Id,
       INSERTED.Name,
       INSERTED.Thumbnail,
       INSERTED.Currency,
       INSERTED.CreatedBy,
       INSERTED.CreatedAtUtc,
       INSERTED.Status
VALUES (@Name, @Thumbnail, @Currency, @CreatedBy, @Status);