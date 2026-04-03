using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.DataAccess.Models;

public class DbEntry
{
    public int Id { get; init; }
    public int SettlementId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int AddedBy { get; init; }
    public DateTime AddedAtUtc { get; init; }
    public EDistributionMode DistributionMode { get; init; }
}
