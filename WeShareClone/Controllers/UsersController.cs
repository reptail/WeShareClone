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
    /// <summary>Returns the profile of the authenticated user.</summary>
    /// <returns>The authenticated user's profile.</returns>
    /// <response code="200">Returns the user's profile.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Authenticated user was not found.</response>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetMeAsync()
    {
        int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

        User? user = await userRepository.GetByIdAsync(userId);
        if (user is null)
            return NotFound();

        return Ok(user.ToDto());
    }

    /// <summary>Updates the profile (name and phone number) of the authenticated user.</summary>
    /// <param name="dto">The updated profile data.</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">Profile updated successfully. Returns the updated user.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Authenticated user was not found.</response>
    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateProfileAsync([FromBody] UpdateProfileDto dto)
    {
        int userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

        User? updated = await userRepository.UpdateProfileAsync(id: userId, name: dto.Name, phone: dto.Phone);
        if (updated is null)
            return NotFound();

        return Ok(updated.ToDto());
    }

    /// <summary>Searches for users by name or email (partial, case-insensitive). Returns up to 10 results.</summary>
    /// <param name="q">Search query matched against name and email.</param>
    /// <returns>Matching users.</returns>
    /// <response code="200">Returns matching users (may be empty).</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("search")]
    [ProducesResponseType<UserDto[]>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto[]>> SearchAsync([FromQuery] string q)
    {
        User[] users = await userRepository.SearchAsync(q);
        return Ok(users.Select(u => u.ToDto()).ToArray());
    }
}
