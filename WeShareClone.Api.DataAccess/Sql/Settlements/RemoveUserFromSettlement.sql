DELETE FROM SettlementUsers
WHERE  SettlementId = @SettlementId
AND    UserId       = @UserId;