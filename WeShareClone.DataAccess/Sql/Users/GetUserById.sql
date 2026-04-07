SELECT Id, Email, Name, Phone, Role, JoinedAtUtc, IsDeleted
FROM Users
WHERE Id = @Id
  AND IsDeleted = 0;