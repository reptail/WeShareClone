INSERT INTO Entries (SettlementId, Name, Value, Currency, AddedBy)
OUTPUT INSERTED.Id,
       INSERTED.SettlementId,
       INSERTED.Name,
       INSERTED.Value,
       INSERTED.Currency,
       INSERTED.AddedBy,
       INSERTED.AddedAtUtc
VALUES (@SettlementId, @Name, @Value, @Currency, @AddedBy);