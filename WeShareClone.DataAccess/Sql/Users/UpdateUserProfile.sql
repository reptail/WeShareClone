UPDATE Users
SET    Name  = @Name,
       Phone = @Phone
OUTPUT INSERTED.Id,
       INSERTED.Email,
       INSERTED.Name,
       INSERTED.Phone,
       INSERTED.Role,
       INSERTED.JoinedAtUtc,
       INSERTED.IsDeleted
WHERE  Id = @Id;
