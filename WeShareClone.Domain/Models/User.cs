namespace WeShareClone.Domain.Models;

public record User(
    int Id,
    string Email,
    string Name,
    string? Phone,
    EUserRole Role,
    DateTime JoinedAtUtc,
    bool IsDeleted);