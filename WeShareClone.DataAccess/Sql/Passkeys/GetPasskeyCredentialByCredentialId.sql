SELECT Id, UserId, CredentialId, PublicKey, SignCount, AaGuid, CreatedAtUtc
FROM PasskeyCredentials
WHERE CredentialId = @CredentialId;