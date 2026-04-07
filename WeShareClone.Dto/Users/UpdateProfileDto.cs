using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Dto.Users;

/// <summary>Request body for updating a user's profile.</summary>
public class UpdateProfileDto
{
    /// <summary>The updated display name.</summary>
    [Required]
    public string Name { get; init; } = string.Empty;

    /// <summary>Optional phone number.</summary>
    public string? Phone { get; init; }

    /// <summary>Initializes a new empty instance of <see cref="UpdateProfileDto"/>.</summary>
    public UpdateProfileDto() { }

    /// <summary>Initializes a new instance of <see cref="UpdateProfileDto"/>.</summary>
    /// <param name="name">The user's display name.</param>
    /// <param name="phone">The user's phone number.</param>
    public UpdateProfileDto(string name, string? phone = null)
    {
        Name  = name;
        Phone = phone;
    }
}
