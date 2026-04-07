UPDATE SettlementDebt
SET    IsPaid       = @IsPaid,
       PaidAtUtc    = CASE WHEN @IsPaid = 1 THEN SYSUTCDATETIME() ELSE NULL END,
       UpdatedAtUtc = SYSUTCDATETIME()
OUTPUT INSERTED.Id,
       INSERTED.SettlementId,
       INSERTED.FromUserId,
       INSERTED.ToUserId,
       INSERTED.Amount,
       INSERTED.Currency,
       INSERTED.IsPaid,
       INSERTED.PaidAtUtc,
       INSERTED.CreatedAtUtc,
       INSERTED.UpdatedAtUtc
WHERE  Id = @Id;
