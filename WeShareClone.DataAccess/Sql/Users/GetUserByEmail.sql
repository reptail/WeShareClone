SELECT Id, Email, Name, Phone, Role, JoinedAtUtc, IsDeleted
FROM Users
WHERE Email = @Email
  AND IsDeleted = 0;
