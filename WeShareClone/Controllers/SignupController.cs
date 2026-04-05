using Microsoft.AspNetCore.Mvc;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Services;
using WeShareClone.Dto.Auth;

namespace WeShareClone.Controllers;

[ApiController]
[Route("api/auth/signup")]
public class SignupController(IAuthService authService) : ControllerBase
{
    /// <summary>Initiates a signup request for the given email address and display name.</summary>
    /// <remarks>
    /// If the email is already registered, this behaves exactly like a login request — a verification
    /// code is sent to the email. If the email is new, a pending signup is recorded and a code is sent.
    /// Always returns 200 to prevent user enumeration.
    /// </remarks>
    /// <param name="dto">The signup request containing the email address and display name.</param>
    /// <response code="200">Request processed successfully.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SignupAsync([FromBody] SignupRequestDto dto)
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
    [HttpPost("verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthTokenDto>> VerifySignupAsync([FromBody] VerifyRequestDto dto)
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
