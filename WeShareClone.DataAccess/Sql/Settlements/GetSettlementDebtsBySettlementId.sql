SELECT sd.Id,
       sd.SettlementId,
       sd.FromUserId,
       sd.ToUserId,
       sd.Amount,
       sd.Currency,
       sd.IsPaid,
       sd.PaidAtUtc,
       sd.CreatedAtUtc,
       sd.UpdatedAtUtc
FROM   SettlementDebt sd
WHERE  sd.SettlementId = @SettlementId
ORDER BY sd.Id;
