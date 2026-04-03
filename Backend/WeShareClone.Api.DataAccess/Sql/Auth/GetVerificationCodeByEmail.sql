SELECT Id, Email, CodeHash, CreatedAtUtc, ExpiresAtUtc
FROM VerificationCodes
WHERE Email = @Email;