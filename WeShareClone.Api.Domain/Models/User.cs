namespace WeShareClone.Api.Domain.Models;

public record User(
    int Id,
    string Email,
    string Name,
    EUserRole Role,
    DateTime JoinedAtUtc,
    bool IsDeleted);