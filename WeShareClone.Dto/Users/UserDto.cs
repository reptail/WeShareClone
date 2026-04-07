namespace WeShareClone.Dto.Users;

public class UserDto
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string Role { get; init; } = string.Empty;

    public UserDto() { }

    public UserDto(int id, string email, string name, string? phone, string role)
    {
        Id    = id;
        Email = email;
        Name  = name;
        Phone = phone;
        Role  = role;
    }
}
