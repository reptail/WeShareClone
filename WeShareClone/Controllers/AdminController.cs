using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WeShareClone.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController(IConfiguration configuration) : ControllerBase
{
    /// <summary>Returns the configured sign-up invite token for constructing invite links.</summary>
    /// <returns>The raw invite token string.</returns>
    /// <response code="200">Returns the invite token.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not an admin.</response>
    /// <response code="404">No invite token is configured.</response>
    [HttpGet("invite-token")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetInviteToken()
    {
        string? token = configuration["SignupSettings:InviteToken"];

        if (string.IsNullOrEmpty(token))
            return NotFound();

        return Ok(token);
    }
}
