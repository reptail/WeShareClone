INSERT INTO SettlementDebt (SettlementId, FromUserId, ToUserId, Amount, Currency)
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
VALUES (@SettlementId, @FromUserId, @ToUserId, @Amount, @Currency);
