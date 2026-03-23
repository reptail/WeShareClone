SELECT Id, Email, Name, Role, JoinedAtUtc, IsDeleted
FROM Users
WHERE Email = @Email
  AND IsDeleted = 0;
