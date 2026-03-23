using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Api.Dto.Auth;

/// <summary>Request body for beginning a passkey login.</summary>
public class PasskeyLoginBeginRequestDto
{
    /// <summary>The email address of the user attempting to log in.</summary>
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    /// <summary>Initializes a new empty instance of <see cref="PasskeyLoginBeginRequestDto"/>.</summary>
    public PasskeyLoginBeginRequestDto() { }

    /// <summary>Initializes a new instance of <see cref="PasskeyLoginBeginRequestDto"/>.</summary>
    /// <param name="email">The user''s email address.</param>
    public PasskeyLoginBeginRequestDto(string email)
    {
        Email = email;
    }
}