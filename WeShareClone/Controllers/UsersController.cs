using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeShareClone.Domain.Models;
using WeShareClone.Domain.Repositories;
using WeShareClone.Dto.Users;
using WeShareClone.Extensions;

namespace WeShareClone.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserRepository userRepository) : ControllerBase
{
    /// <summary>Updates the display name of the authenticated user.</summary>
    /// <param name="name">The new display name.</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">Name updated successfully. Returns the updated user.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Authenticated user was not found.</response>
    [HttpPut("me/name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateNameAsync([FromBody] string name)
    {
        int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

        User? updated = await userRepository.UpdateNameAsync(id: userId, name: name);
        if (updated is null)
            return NotFound();

        return Ok(updated.ToDto());
    }
}
