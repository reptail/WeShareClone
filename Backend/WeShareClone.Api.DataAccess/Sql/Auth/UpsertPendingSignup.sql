DELETE FROM PendingSignups
WHERE Email = @Email;

INSERT INTO PendingSignups (Email, Name)
VALUES (@Email, @Name);