using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;
using WeShareClone.Domain.Services;

namespace WeShareClone.Services;

public class AuthService(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPendingSignupRepository pendingSignupRepository,
    IConfiguration configuration) : IAuthService
{
    private const int VERIFICATION_CODE_EXPIRY_MINUTES = 10;
    private const int REFRESH_TOKEN_EXPIRY_DAYS = 30;

    public async Task RequestLoginAsync(string email)
    {
        User? user = await userRepository.GetByEmailAsync(email);
        if (user is null)
            return;

        await GenerateAndSendVerificationCodeAsync(email);
    }

    public async Task<AuthToken?> VerifyCodeAsync(string email, string code)
    {
        VerificationCode? stored = await verificationCodeRepository.GetByEmailAsync(email);
        if (stored is null)
            return null;

        if (stored.ExpiresAtUtc < DateTime.UtcNow)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(text: code, hash: stored.CodeHash))
            return null;

        await verificationCodeRepository.DeleteByEmailAsync(email);

        User? user = await userRepository.GetByEmailAsync(email);
        if (user is null)
            return null;

        return await IssueTokensAsync(user);
    }

    public async Task<AuthToken?> RefreshTokenAsync(string refreshToken)
    {
        RefreshToken? stored = await refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (stored is null)
            return null;

        if (stored.ExpiresAtUtc < DateTime.UtcNow)
            return null;

        User? user = await userRepository.GetByIdAsync(stored.UserId);
        if (user is null)
            return null;

        await refreshTokenRepository.DeleteByTokenAsync(refreshToken);

        (string accessToken, DateTime expiresAtUtc) = GenerateAccessToken(user);

        string newRefreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        DateTime refreshExpiresAtUtc = DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRY_DAYS);

        await refreshTokenRepository.CreateAsync(
            userId: user.Id,
            token: newRefreshTokenValue,
            expiresAtUtc: refreshExpiresAtUtc
        );

        return new AuthToken(
            AccessToken: accessToken,
            RefreshToken: newRefreshTokenValue,
            ExpiresAtUtc: expiresAtUtc
        );
    }

    public async Task RequestSignupAsync(string email, string name, string? phone = null)
    {
        User? existingUser = await userRepository.GetByEmailAsync(email);
        if (existingUser is null)
            await pendingSignupRepository.UpsertAsync(email: email, name: name, phone: phone);

        await GenerateAndSendVerificationCodeAsync(email);
    }

    public async Task<AuthToken?> VerifySignupAsync(string email, string code)
    {
        VerificationCode? stored = await verificationCodeRepository.GetByEmailAsync(email);
        if (stored is null)
            return null;

        if (stored.ExpiresAtUtc < DateTime.UtcNow)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(text: code, hash: stored.CodeHash))
            return null;

        await verificationCodeRepository.DeleteByEmailAsync(email);

        User? user = await userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            PendingSignup? pending = await pendingSignupRepository.GetByEmailAsync(email);
            if (pending is null)
                return null;

            user = await userRepository.CreateAsync(email: email, name: pending.Name, phone: pending.Phone);
            await pendingSignupRepository.DeleteByEmailAsync(email);
        }

        return await IssueTokensAsync(user);
    }

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

    private async Task GenerateAndSendVerificationCodeAsync(string email)
    {
        // TODO: Remove — fixed dev code, replace with: int code = Random.Shared.Next(100_000, 1_000_000);
        const string codeString = "123456";
        string codeHash = BCrypt.Net.BCrypt.HashPassword(codeString);
        DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(VERIFICATION_CODE_EXPIRY_MINUTES);

        await verificationCodeRepository.UpsertAsync(
            email: email,
            codeHash: codeHash,
            expiresAtUtc: expiresAtUtc
        );

        Debug.WriteLine($"[WeShareClone] Verification code for {email}: {codeString}");
    }

    private (string token, DateTime expiresAtUtc) GenerateAccessToken(User user)
    {
        string key = configuration["JwtSettings:Key"]!;
        string issuer = configuration["JwtSettings:Issuer"]!;
        string audience = configuration["JwtSettings:Audience"]!;
        int expiryMinutes = int.Parse(configuration["JwtSettings:ExpiryMinutes"]!);

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));
        SigningCredentials credentials = new(key: securityKey, algorithm: SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new Claim(type: JwtRegisteredClaimNames.Sub, value: user.Id.ToString()),
            new Claim(type: JwtRegisteredClaimNames.Email, value: user.Email),
            new Claim(type: JwtRegisteredClaimNames.Name, value: user.Name),
            new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
            new Claim(type: ClaimTypes.Role, value: user.Role.ToString()),
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
