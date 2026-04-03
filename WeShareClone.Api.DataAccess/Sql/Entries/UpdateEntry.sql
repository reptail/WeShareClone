UPDATE Entries
SET    Name             = @Name,
       Value            = @Value,
       Currency         = @Currency,
       DistributionMode = @DistributionMode
OUTPUT INSERTED.Id,
       INSERTED.SettlementId,
       INSERTED.Name,
       INSERTED.Value,
       INSERTED.Currency,
       INSERTED.AddedBy,
       INSERTED.AddedAtUtc,
       INSERTED.DistributionMode
WHERE  Id = @Id;