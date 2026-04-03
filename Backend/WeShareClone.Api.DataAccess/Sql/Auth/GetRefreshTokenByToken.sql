SELECT Id, UserId, Token, CreatedAtUtc, ExpiresAtUtc
FROM RefreshTokens
WHERE Token = @Token;