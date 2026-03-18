UPDATE Entries
SET    Name     = @Name,
       Value    = @Value,
       Currency = @Currency
OUTPUT INSERTED.Id,
       INSERTED.SettlementId,
       INSERTED.Name,
       INSERTED.Value,
       INSERTED.Currency,
       INSERTED.AddedBy,
       INSERTED.AddedAtUtc
WHERE  Id = @Id;