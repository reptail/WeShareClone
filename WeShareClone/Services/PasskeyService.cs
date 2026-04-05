using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.IdentityModel.Tokens;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;

namespace WeShareClone.Services;

public class PasskeyService(
    Fido2 fido2,
    IUserRepository userRepository,
    IPasskeyCredentialRepository credentialRepository,
    IPasskeyChallengeRepository challengeRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IConfiguration configuration) : IPasskeyService
{
    private const int CHALLENGE_EXPIRY_MINUTES = 5;
    private const int REFRESH_TOKEN_EXPIRY_DAYS = 30;

    public async Task<CredentialCreateOptions> BeginRegistrationAsync(int userId, string email, string name)
    {
        PasskeyCredential[] existingCredentials = await credentialRepository.GetByUserIdAsync(userId);
        List<PublicKeyCredentialDescriptor> excludeCredentials = existingCredentials
            .Select(static c => new PublicKeyCredentialDescriptor(c.CredentialId))
            .ToList();

        Fido2User user = new()
        {
            Id = BitConverter.GetBytes(userId),
            Name = email,
            DisplayName = name,
        };

        CredentialCreateOptions options = fido2.RequestNewCredential(
            new RequestNewCredentialParams
            {
                User = user,
                ExcludeCredentials = excludeCredentials,
                AuthenticatorSelection = new AuthenticatorSelection
                {
                    ResidentKey = ResidentKeyRequirement.Preferred,
                    UserVerification = UserVerificationRequirement.Preferred,
                },
                AttestationPreference = AttestationConveyancePreference.None,
            }
        );

        await challengeRepository.UpsertAsync(
            email: email,
            type: EPasskeyChallengeType.Registration,
            optionsJson: options.ToJson(),
            expiresAtUtc: DateTime.UtcNow.AddMinutes(CHALLENGE_EXPIRY_MINUTES)
        );

        return options;
    }

    public async Task CompleteRegistrationAsync(int userId, string email, AuthenticatorAttestationRawResponse attestationResponse)
    {
        PasskeyChallenge? challenge = await challengeRepository.GetByEmailAndTypeAsync(
            email: email,
            type: EPasskeyChallengeType.Registration
        );

        if (challenge is null || challenge.ExpiresAtUtc < DateTime.UtcNow)
            throw new InvalidOperationException("Registration challenge not found or expired.");

        CredentialCreateOptions originalOptions = CredentialCreateOptions.FromJson(challenge.OptionsJson);

        RegisteredPublicKeyCredential credential = await fido2.MakeNewCredentialAsync(
            new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = originalOptions,
                IsCredentialIdUniqueToUserCallback = async (parameters, _) =>
                {
                    PasskeyCredential? existing = await credentialRepository.GetByCredentialIdAsync(parameters.CredentialId);
                    return existing is null;
                },
            }
        );

        await credentialRepository.CreateAsync(
            userId: userId,
            credentialId: credential.Id,
            publicKey: credential.PublicKey,
            signCount: (long)credential.SignCount,
            aaGuid: credential.AaGuid
        );

        await challengeRepository.DeleteByEmailAndTypeAsync(
            email: email,
            type: EPasskeyChallengeType.Registration
        );
    }

    public async Task<AssertionOptions?> BeginLoginAsync(string email)
    {
        User? user = await userRepository.GetByEmailAsync(email);
        if (user is null)
            return null;

        PasskeyCredential[] credentials = await credentialRepository.GetByUserIdAsync(user.Id);
        if (credentials.Length == 0)
            return null;

        List<PublicKeyCredentialDescriptor> allowedCredentials = credentials
            .Select(static c => new PublicKeyCredentialDescriptor(c.CredentialId))
            .ToList();

        AssertionOptions options = fido2.GetAssertionOptions(
            new GetAssertionOptionsParams
            {
                AllowedCredentials = allowedCredentials,
                UserVerification = UserVerificationRequirement.Preferred,
            }
        );

        await challengeRepository.UpsertAsync(
            email: email,
            type: EPasskeyChallengeType.Authentication,
            optionsJson: options.ToJson(),
            expiresAtUtc: DateTime.UtcNow.AddMinutes(CHALLENGE_EXPIRY_MINUTES)
        );

        return options;
    }

    public async Task<AuthToken?> CompleteLoginAsync(AuthenticatorAssertionRawResponse assertionResponse)
    {
        PasskeyCredential? storedCredential = await credentialRepository.GetByCredentialIdAsync(assertionResponse.RawId);
        if (storedCredential is null)
            return null;

        User? user = await userRepository.GetByIdAsync(storedCredential.UserId);
        if (user is null)
            return null;

        PasskeyChallenge? challenge = await challengeRepository.GetByEmailAndTypeAsync(
            email: user.Email,
            type: EPasskeyChallengeType.Authentication
        );

        if (challenge is null || challenge.ExpiresAtUtc < DateTime.UtcNow)
            return null;

        AssertionOptions originalOptions = AssertionOptions.FromJson(challenge.OptionsJson);

        VerifyAssertionResult result = await fido2.MakeAssertionAsync(
            new MakeAssertionParams
            {
                AssertionResponse = assertionResponse,
                OriginalOptions = originalOptions,
                StoredPublicKey = storedCredential.PublicKey,
                StoredSignatureCounter = (uint)storedCredential.SignCount,
                IsUserHandleOwnerOfCredentialIdCallback = (parameters, _) =>
                    Task.FromResult(
                        parameters.CredentialId.SequenceEqual(storedCredential.CredentialId)
                    ),
            }
        );

        await credentialRepository.UpdateSignCountAsync(
            id: storedCredential.Id,
            signCount: (long)result.SignCount
        );

        await challengeRepository.DeleteByEmailAndTypeAsync(
            email: user.Email,
            type: EPasskeyChallengeType.Authentication
        );

        return await IssueTokensAsync(user);
    }

    public Task<PasskeyCredential[]> GetCredentialsByUserIdAsync(int userId)
        => credentialRepository.GetByUserIdAsync(userId);

    public Task DeleteCredentialAsync(int userId, int credentialId)
        => credentialRepository.DeleteByIdAsync(id: credentialId, userId: userId);

    private async Task<AuthToken> IssueTokensAsync(User user)
    {
        (string accessToken, DateTime expiresAtUtc) = GenerateAccessToken(user);

        string refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        DateTime refreshExpiresAtUtc = DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRY_DAYS);

        await refreshTokenRepository.CreateAsync(
            userId: user.Id,
            token: refreshTokenValue,
            expiresAtUtc: refreshExpiresAtUtc
        );

        return new AuthToken(
            AccessToken: accessToken,
            RefreshToken: refreshTokenValue,
            ExpiresAtUtc: expiresAtUtc
        );
    }

    private (string token, DateTime expiresAtUtc) GenerateAccessToken(User user)
    {
        string key = configuration["JwtSettings:Key"]!;
        string issuer = configuration["JwtSettings:Issuer"]!;
        string audience = configuration["JwtSettings:Audience"]!;
        int expiryMinutes = int.Parse(configuration["JwtSettings:ExpiryMinutes"]!);

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        ];

        DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}