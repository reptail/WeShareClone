INSERT INTO RefreshTokens (UserId, Token, ExpiresAtUtc)
OUTPUT INSERTED.*
VALUES (@UserId, @Token, @ExpiresAtUtc);