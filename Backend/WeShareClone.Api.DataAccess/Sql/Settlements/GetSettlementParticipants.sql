SELECT u.Id,
       u.Email,
       u.Name,
       u.Role,
       u.JoinedAtUtc,
       u.IsDeleted
FROM   Users u
WHERE  u.Id = (SELECT CreatedBy FROM Settlements WHERE Id = @SettlementId)
   AND u.IsDeleted = 0
UNION
SELECT u.Id,
       u.Email,
       u.Name,
       u.Role,
       u.JoinedAtUtc,
       u.IsDeleted
FROM   Users u
INNER JOIN SettlementUsers su ON su.UserId = u.Id
WHERE  su.SettlementId = @SettlementId
   AND u.IsDeleted = 0;
