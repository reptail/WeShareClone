using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Dto.Auth;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    public LoginRequestDto() { }

    public LoginRequestDto(string email)
    {
        Email = email;
    }
}