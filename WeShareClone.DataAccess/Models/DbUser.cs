using WeShareClone.Domain.Models;

namespace WeShareClone.DataAccess.Models;

public class DbUser
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public EUserRole Role { get; init; }
    public DateTime JoinedAtUtc { get; init; }
    public bool IsDeleted { get; init; }
}
