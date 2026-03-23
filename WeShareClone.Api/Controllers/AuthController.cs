using Microsoft.AspNetCore.Mvc;
using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Domain.Services;
using WeShareClone.Api.Dto.Auth;

namespace WeShareClone.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Initiates a login request for the given email address.</summary>
    /// <remarks>
    /// If the email belongs to a registered account, a 6-digit verification code is generated and
    /// sent to the user. If no account exists for the email, the request silently succeeds without
    /// any action, to prevent user enumeration.
    /// </remarks>
    /// <param name="dto">The login request containing the user email address.</param>
    /// <response code="200">Request processed successfully.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        await authService.RequestLoginAsync(dto.Email);
        return Ok();
    }

    /// <summary>Verifies a login code and returns a JWT access token with refresh token.</summary>
    /// <param name="dto">The verification request containing the email and 6-digit code.</param>
    /// <returns>A JWT access token and refresh token on success.</returns>
    /// <response code="200">Code verified successfully. Returns access and refresh tokens.</response>
    /// <response code="401">The verification code is invalid or has expired.</response>
    [HttpPost("verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthTokenDto>> Verify([FromBody] VerifyRequestDto dto)
    {
        AuthToken? token = await authService.VerifyCodeAsync(
            email: dto.Email,
            code: dto.Code
        );

        if (token is null)
            return Unauthorized();

        return Ok(new AuthTokenDto(
            accessToken: token.AccessToken,
            refreshToken: token.RefreshToken,
            expiresAtUtc: token.ExpiresAtUtc
        ));
    }

    /// <summary>Issues a new access token using a valid refresh token.</summary>
    /// <remarks>
    /// The submitted refresh token is rotated on success — a new refresh token is returned and the
    /// previous one is invalidated. Store the new refresh token for subsequent calls.
    /// </remarks>
    /// <param name="dto">The refresh request containing the current refresh token.</param>
    /// <returns>A new JWT access token and rotated refresh token.</returns>
    /// <response code="200">Token refreshed successfully. Returns new access and refresh tokens.</response>
    /// <response code="401">The refresh token is invalid or has expired.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthTokenDto>> Refresh([FromBody] RefreshRequestDto dto)
    {
        AuthToken? token = await authService.RefreshTokenAsync(dto.RefreshToken);

        if (token is null)
            return Unauthorized();

        return Ok(new AuthTokenDto(
            accessToken: token.AccessToken,
            refreshToken: token.RefreshToken,
            expiresAtUtc: token.ExpiresAtUtc
        ));
    }

    /// <summary>Initiates a signup request for the given email address and display name.</summary>
    /// <remarks>
    /// If the email is already registered, this behaves exactly like a login request — a verification
    /// code is sent to the email. If the email is new, a pending signup is recorded and a code is sent.
    /// Always returns 200 to prevent user enumeration.
    /// </remarks>
    /// <param name="dto">The signup request containing the email address and display name.</param>
    /// <response code="200">Request processed successfully.</response>
    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Signup([FromBody] SignupRequestDto dto)
    {
        await authService.RequestSignupAsync(
            email: dto.Email,
            name: dto.Name
        );
        return Ok();
    }

    /// <summary>Verifies a signup code and returns a JWT access token with refresh token.</summary>
    /// <remarks>
    /// If the email belongs to an existing user, this completes a login. If the email belongs to a
    /// pending signup, the user account is created and an auth token is returned.
    /// </remarks>
    /// <param name="dto">The verification request containing the email and 6-digit code.</param>
    /// <returns>A JWT access token and refresh token on success.</returns>
    /// <response code="200">Code verified successfully. Returns access and refresh tokens.</response>
    /// <response code="401">The verification code is invalid, expired, or no pending signup/user found.</response>
    [HttpPost("signup/verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthTokenDto>> VerifySignup([FromBody] VerifyRequestDto dto)
    {
        AuthToken? token = await authService.VerifySignupAsync(
            email: dto.Email,
            code: dto.Code
        );

        if (token is null)
            return Unauthorized();

        return Ok(new AuthTokenDto(
            accessToken: token.AccessToken,
            refreshToken: token.RefreshToken,
            expiresAtUtc: token.ExpiresAtUtc
        ));
    }
}