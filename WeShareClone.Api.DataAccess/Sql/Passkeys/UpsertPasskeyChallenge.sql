DELETE FROM PasskeyChallenges
WHERE Email = @Email
  AND ChallengeType = @ChallengeType;

INSERT INTO PasskeyChallenges (Email, ChallengeType, OptionsJson, ExpiresAtUtc)
VALUES (@Email, @ChallengeType, @OptionsJson, @ExpiresAtUtc);