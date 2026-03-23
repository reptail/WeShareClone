SELECT Id, Email, Name, CreatedAtUtc
FROM PendingSignups
WHERE Email = @Email;