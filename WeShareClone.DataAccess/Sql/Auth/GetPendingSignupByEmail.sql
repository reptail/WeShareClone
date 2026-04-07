SELECT Id, Email, Name, Phone, CreatedAtUtc
FROM PendingSignups
WHERE Email = @Email;