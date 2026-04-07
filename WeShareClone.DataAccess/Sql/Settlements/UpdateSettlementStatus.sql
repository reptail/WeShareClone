UPDATE Settlements
SET    Status = @Status
OUTPUT INSERTED.Id,
       INSERTED.Name,
       INSERTED.Thumbnail,
       INSERTED.Currency,
       INSERTED.CreatedBy,
       INSERTED.CreatedAtUtc,
       INSERTED.Status
WHERE  Id = @Id;
