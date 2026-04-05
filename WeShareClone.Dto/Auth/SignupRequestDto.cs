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

    /// <summary>Initializes a new empty instance of <see cref="SignupRequestDto"/>.</summary>
    public SignupRequestDto() { }

    /// <summary>Initializes a new instance of <see cref="SignupRequestDto"/>.</summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="name">The user's display name.</param>
    public SignupRequestDto(string email, string name)
    {
        Email = email;
        Name = name;
    }
}