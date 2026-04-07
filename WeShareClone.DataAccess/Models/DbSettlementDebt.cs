namespace WeShareClone.DataAccess.Models;

public class DbSettlementDebt
{
    public int Id { get; init; }
    public int SettlementId { get; init; }
    public int FromUserId { get; init; }
    public int ToUserId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public bool IsPaid { get; init; }
    public DateTime? PaidAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}
