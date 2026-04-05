DELETE FROM PasskeyChallenges
WHERE Email = @Email
  AND ChallengeType = @ChallengeType;