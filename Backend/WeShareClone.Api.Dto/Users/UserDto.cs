namespace WeShareClone.Api.Dto.Users;

public class UserDto
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;

    public UserDto() { }

    public UserDto(int id, string email, string name, string role)
    {
        Id = id;
        Email = email;
        Name = name;
        Role = role;
    }
}
