SELECT Id, UserId, CredentialId, PublicKey, SignCount, AaGuid, Name, CreatedAtUtc
FROM PasskeyCredentials
WHERE UserId = @UserId;