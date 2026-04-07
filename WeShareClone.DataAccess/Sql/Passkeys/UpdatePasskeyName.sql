-- Update the user-defined name of a passkey credential.
-- Scoped to the owning user to prevent cross-user modifications.
UPDATE PasskeyCredentials
SET Name = @Name
OUTPUT INSERTED.Id,
       INSERTED.UserId,
       INSERTED.CredentialId,
       INSERTED.PublicKey,
       INSERTED.SignCount,
       INSERTED.AaGuid,
       INSERTED.Name,
       INSERTED.CreatedAtUtc
WHERE Id     = @Id
  AND UserId = @UserId;
