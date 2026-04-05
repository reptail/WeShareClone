using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Dto.Auth;

public class RefreshRequestDto
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;

    public RefreshRequestDto() { }

    public RefreshRequestDto(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}