namespace WeShareClone.Web.Models;

public class SettlementDebtModel
{
    public int Id { get; set; }
    public int SettlementId { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
