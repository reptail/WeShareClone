using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Dto.Auth;

public class VerifyRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string Code { get; init; } = string.Empty;

    public VerifyRequestDto() { }

    public VerifyRequestDto(string email, string code)
    {
        Email = email;
        Code = code;
    }
}