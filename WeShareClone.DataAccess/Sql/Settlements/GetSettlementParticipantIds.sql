SELECT UserId
FROM   SettlementUsers
WHERE  SettlementId = @SettlementId
UNION
SELECT CreatedBy
FROM   Settlements
WHERE  Id = @SettlementId;
