INSERT INTO Entries (SettlementId, Name, Value, Currency, AddedBy, DistributionMode)
OUTPUT INSERTED.Id,
       INSERTED.SettlementId,
       INSERTED.Name,
       INSERTED.Value,
       INSERTED.Currency,
       INSERTED.AddedBy,
       INSERTED.AddedAtUtc,
       INSERTED.DistributionMode
VALUES (@SettlementId, @Name, @Value, @Currency, @AddedBy, @DistributionMode);