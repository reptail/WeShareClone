DELETE FROM PendingSignups
WHERE Email = @Email;

INSERT INTO PendingSignups (Email, Name, Phone)
VALUES (@Email, @Name, @Phone);