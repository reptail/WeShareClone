namespace WeShareClone.Domain.Models;

public record Settlement(
    int Id,
    string Name,
    string? Thumbnail,
    string Currency,
    int CreatedBy,
    DateTime CreatedAtUtc,
    ESettlementStatus Status);