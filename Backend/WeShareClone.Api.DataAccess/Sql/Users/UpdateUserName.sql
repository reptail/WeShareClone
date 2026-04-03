UPDATE Users
SET    Name = @Name
OUTPUT INSERTED.Id,
       INSERTED.Email,
       INSERTED.Name,
       INSERTED.Role,
       INSERTED.JoinedAtUtc,
       INSERTED.IsDeleted
WHERE  Id = @Id;
