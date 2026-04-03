using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Api.Dto.Entries;

public class EntryDto
{
    public int Id { get; init; }
    public int SettlementId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Value { get; init; }
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = string.Empty;
    public int AddedBy { get; init; }
    public DateTime AddedAtUtc { get; init; }
    public EntryDistributionDto[] Distributions { get; init; } = [];
    public string DistributionMode { get; init; } = string.Empty;

    public EntryDto() { }

    public EntryDto(
        int id,
        int settlementId,
        string name,
        decimal value,
        string currency,
        int addedBy,
        DateTime addedAtUtc,
        EntryDistributionDto[] distributions,
        string distributionMode)
    {
        Id = id;
        SettlementId = settlementId;
        Name = name;
        Value = value;
        Currency = currency;
        AddedBy = addedBy;
        AddedAtUtc = addedAtUtc;
        Distributions = distributions;
        DistributionMode = distributionMode;
    }
}
