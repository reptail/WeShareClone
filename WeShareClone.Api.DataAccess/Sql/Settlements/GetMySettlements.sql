SELECT s.Id,
       s.Name,
       s.Thumbnail,
       s.Currency,
       s.CreatedBy,
       s.CreatedAtUtc,
       s.IsOpen
FROM   Settlements s
WHERE  s.CreatedBy = @UserId
UNION
SELECT s.Id,
       s.Name,
       s.Thumbnail,
       s.Currency,
       s.CreatedBy,
       s.CreatedAtUtc,
       s.IsOpen
FROM   Settlements s
INNER JOIN SettlementUsers su ON su.SettlementId = s.Id
WHERE  su.UserId = @UserId
ORDER BY Name;
