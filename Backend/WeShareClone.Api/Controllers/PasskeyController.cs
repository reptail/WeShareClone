using System.IdentityModel.Tokens.Jwt;
using Fido2NetLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Dto.Auth;
using WeShareClone.Api.Services;

namespace WeShareClone.Api.Controllers;

[ApiController]
[Route("auth/passkey")]
public class PasskeyController(IPasskeyService passkeyService) : ControllerBase
{
    /// <summary>Begins passkey registration by returning credential creation options.</summary>
    /// <remarks>
    /// The returned <c>CredentialCreateOptions</c> should be passed to <c>navigator.credentials.create()</c>
    /// in the browser. The response must be submitted to <c>POST /auth/passkey/register/complete</c>
    /// within 5 minutes.
    /// </remarks>
    /// <returns>WebAuthn credential creation options.</returns>
    /// <response code="200">Returns credential creation options for the client.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("register/begin")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CredentialCreateOptions>> BeginRegistrationAsync()
    {
        int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        string email = User.FindFirst(JwtRegisteredClaimNames.Email)!.Value;
        string name = User.FindFirst(JwtRegisteredClaimNames.Name)!.Value;

        CredentialCreateOptions options = await passkeyService.BeginRegistrationAsync(
            userId: userId,
            email: email,
            name: name
        );

        return Ok(options);
    }

    /// <summary>Completes passkey registration by validating and storing the new credential.</summary>
    /// <remarks>
    /// Submit the <c>AuthenticatorAttestationRawResponse</c> returned by <c>navigator.credentials.create()</c>
    /// to register the passkey.
    /// </remarks>
    /// <param name="attestationResponse">The attestation response from the authenticator.</param>
    /// <response code="204">Passkey registered successfully.</response>
    /// <response code="400">The attestation response is invalid or the registration challenge has expired.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("register/complete")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CompleteRegistrationAsync([FromBody] AuthenticatorAttestationRawResponse attestationResponse)
    {
        int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        string email = User.FindFirst(JwtRegisteredClaimNames.Email)!.Value;

        try
        {
            await passkeyService.CompleteRegistrationAsync(
                userId: userId,
                email: email,
                attestationResponse: attestationResponse
            );
            return NoContent();
        }
        catch (Fido2VerificationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Begins a passkey login by returning assertion options for the given email.</summary>
    /// <remarks>
    /// The returned <c>AssertionOptions</c> should be passed to <c>navigator.credentials.get()</c>
    /// in the browser. The response must be submitted to <c>POST /auth/passkey/login/complete</c>
    /// within 5 minutes.
    /// </remarks>
    /// <param name="dto">The login request containing the user''s email address.</param>
    /// <returns>WebAuthn assertion options, or 404 if no passkeys are registered for this email.</returns>
    /// <response code="200">Returns assertion options for the client.</response>
    /// <response code="404">No passkeys found for the given email address.</response>
    [HttpPost("login/begin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssertionOptions>> BeginLoginAsync([FromBody] PasskeyLoginBeginRequestDto dto)
    {
        AssertionOptions? options = await passkeyService.BeginLoginAsync(dto.Email);

        if (options is null)
            return NotFound();

        return Ok(options);
    }

    /// <summary>Completes a passkey login by verifying the assertion and returning a JWT token.</summary>
    /// <remarks>
    /// Submit the <c>AuthenticatorAssertionRawResponse</c> returned by <c>navigator.credentials.get()</c>
    /// to authenticate. Returns a JWT access token and a refresh token on success.
    /// </remarks>
    /// <param name="assertionResponse">The assertion response from the authenticator.</param>
    /// <returns>A JWT access token and refresh token on success.</returns>
    /// <response code="200">Passkey verified. Returns access and refresh tokens.</response>
    /// <response code="401">The assertion is invalid, the credential was not found, or the challenge has expired.</response>
    [HttpPost("login/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthTokenDto>> CompleteLoginAsync([FromBody] AuthenticatorAssertionRawResponse assertionResponse)
    {
        AuthToken? token = await passkeyService.CompleteLoginAsync(assertionResponse);

        if (token is null)
            return Unauthorized();

        return Ok(new AuthTokenDto(
            accessToken: token.AccessToken,
            refreshToken: token.RefreshToken,
            expiresAtUtc: token.ExpiresAtUtc
        ));
    }

    /// <summary>Returns all passkeys registered for the authenticated user.</summary>
    /// <returns>A list of registered passkey credentials.</returns>
    /// <response code="200">Returns the list of registered passkeys.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("credentials")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PasskeyCredentialDto[]>> GetCredentialsAsync()
    {
        int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        PasskeyCredential[] credentials = await passkeyService.GetCredentialsByUserIdAsync(userId);

        PasskeyCredentialDto[] dtos = credentials
            .Select(static c => new PasskeyCredentialDto(
                id: c.Id,
                aaGuid: c.AaGuid,
                createdAtUtc: c.CreatedAtUtc
            ))
            .ToArray();

        return Ok(dtos);
    }

    /// <summary>Deletes a registered passkey credential.</summary>
    /// <param name="id">The database identifier of the passkey to remove.</param>
    /// <response code="204">Passkey deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpDelete("credentials/{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteCredentialAsync([FromRoute] int id)
    {
        int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        await passkeyService.DeleteCredentialAsync(userId: userId, credentialId: id);
        return NoContent();
    }
}