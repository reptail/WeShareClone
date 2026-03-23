SELECT Id, Email, Name, Role, JoinedAtUtc, IsDeleted
FROM Users
WHERE Id = @Id
  AND IsDeleted = 0;