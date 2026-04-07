namespace WeShareClone.Dto.Settlements;

public class SettlementDebtDto
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

    public SettlementDebtDto() { }

    public SettlementDebtDto(
        int id,
        int settlementId,
        int fromUserId,
        int toUserId,
        decimal amount,
        string currency,
        bool isPaid,
        DateTime? paidAtUtc,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        Id = id;
        SettlementId = settlementId;
        FromUserId = fromUserId;
        ToUserId = toUserId;
        Amount = amount;
        Currency = currency;
        IsPaid = isPaid;
        PaidAtUtc = paidAtUtc;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }
}
