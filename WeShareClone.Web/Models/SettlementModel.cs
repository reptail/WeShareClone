namespace WeShareClone.Web.Models;

public class SettlementModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int Status { get; set; }

    public ESettlementStatus SettlementStatus => (ESettlementStatus)Status;
    public bool IsOpen => SettlementStatus == ESettlementStatus.Open;
}
