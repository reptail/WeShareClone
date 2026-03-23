DELETE FROM VerificationCodes
WHERE Email = @Email;

INSERT INTO VerificationCodes (Email, CodeHash, ExpiresAtUtc)
VALUES (@Email, @CodeHash, @ExpiresAtUtc);