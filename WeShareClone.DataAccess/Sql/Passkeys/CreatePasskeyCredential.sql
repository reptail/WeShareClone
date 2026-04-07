INSERT INTO PasskeyCredentials (UserId, CredentialId, PublicKey, SignCount, AaGuid)
OUTPUT INSERTED.Id,
       INSERTED.UserId,
       INSERTED.CredentialId,
       INSERTED.PublicKey,
       INSERTED.SignCount,
       INSERTED.AaGuid,
       INSERTED.Name,
       INSERTED.CreatedAtUtc
VALUES (@UserId, @CredentialId, @PublicKey, @SignCount, @AaGuid);