namespace WeShareClone.Domain.Models;

public record SettlementDebt(
    int Id,
    int SettlementId,
    int FromUserId,
    int ToUserId,
    decimal Amount,
    string Currency,
    bool IsPaid,
    DateTime? PaidAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
