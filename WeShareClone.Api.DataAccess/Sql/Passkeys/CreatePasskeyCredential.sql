INSERT INTO PasskeyCredentials (UserId, CredentialId, PublicKey, SignCount, AaGuid)
OUTPUT INSERTED.*
VALUES (@UserId, @CredentialId, @PublicKey, @SignCount, @AaGuid);