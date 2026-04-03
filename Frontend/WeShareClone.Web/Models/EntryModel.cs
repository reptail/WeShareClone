namespace WeShareClone.Web.Models;

public class EntryModel
{
    public int Id { get; set; }
    public int SettlementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int AddedBy { get; set; }
    public DateTime AddedAtUtc { get; set; }
    public string DistributionMode { get; set; } = string.Empty;
    public EntryDistributionModel[] Distributions { get; set; } = [];
}

public class EntryDistributionModel
{
    public int UserId { get; set; }
    public decimal Factor { get; set; }
}
