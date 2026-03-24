SELECT CASE
    WHEN EXISTS (
        SELECT 1 FROM SettlementUsers WHERE SettlementId = @SettlementId AND UserId = @UserId
        UNION ALL
        SELECT 1 FROM Settlements      WHERE Id           = @SettlementId AND CreatedBy = @UserId
    ) THEN 1 ELSE 0
END;
