DELETE FROM PasskeyCredentials
WHERE Id = @Id
  AND UserId = @UserId;