-- Returns up to 10 active users whose name or email contains @Query (case-insensitive).
SELECT TOP 10
    u.Id,
    u.Email,
    u.Name,
    u.Role,
    u.JoinedAtUtc,
    u.IsDeleted
FROM Users u
WHERE u.IsDeleted = 0
  AND (
        u.Email LIKE '%' + @Query + '%'
     OR u.Name  LIKE '%' + @Query + '%'
  )
ORDER BY u.Name;
