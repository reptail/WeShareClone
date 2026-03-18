namespace WeShareClone.Api.Domain.Models;

public record User(
    int Id,
    string Email,
    string Name,
    string? Thumbnail,
    DateTime JoinedAtUtc,
    bool IsDeleted);