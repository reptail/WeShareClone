SELECT Id, Email, ChallengeType, OptionsJson, ExpiresAtUtc
FROM PasskeyChallenges
WHERE Email = @Email
  AND ChallengeType = @ChallengeType;