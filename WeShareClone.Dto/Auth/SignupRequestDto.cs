using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Dto.Auth;

/// <summary>Request body for initiating a signup.</summary>
public class SignupRequestDto
{
    /// <summary>The email address of the user signing up.</summary>
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    /// <summary>The display name of the user.</summary>
    [Required]
    public string Name { get; init; } = string.Empty;

    /// <summary>Optional invite token. Required when the API is configured for invite-only sign-up.</summary>
    public string? InviteToken { get; init; }

    /// <summary>Initializes a new empty instance of <see cref="SignupRequestDto"/>.</summary>
    public SignupRequestDto() { }

    /// <summary>Initializes a new instance of <see cref="SignupRequestDto"/>.</summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="name">The user's display name.</param>
    /// <param name="inviteToken">Optional invite token.</param>
    public SignupRequestDto(string email, string name, string? inviteToken = null)
    {
        Email       = email;
        Name        = name;
        InviteToken = inviteToken;
    }
}