namespace WeShareClone.Api.Domain.Models;

public record Entry(
    int Id,
    int SettlementId,
    string Name,
    decimal Value,
    string Currency,
    int AddedBy,
    DateTime AddedAtUtc);