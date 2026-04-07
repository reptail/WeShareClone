using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Services;

public interface IAuthService
{
    /// <summary>Initiates login for the given email. Silently no-ops if the user does not exist.</summary>
    Task RequestLoginAsync(string email);

    /// <summary>Verifies the submitted code and returns an AuthToken, or null if invalid or expired.</summary>
    Task<AuthToken?> VerifyCodeAsync(string email, string code);

    /// <summary>Issues a new AuthToken using a valid refresh token, rotating the refresh token. Returns null if invalid or expired.</summary>
    Task<AuthToken?> RefreshTokenAsync(string refreshToken);

    /// <summary>Initiates signup for the given email and name. If email is already registered, acts like login.</summary>
    Task RequestSignupAsync(string email, string name, string? phone = null);

    /// <summary>Verifies the code and completes signup or login. Returns null if code is invalid, expired, or no pending signup/user found.</summary>
    Task<AuthToken?> VerifySignupAsync(string email, string code);
}