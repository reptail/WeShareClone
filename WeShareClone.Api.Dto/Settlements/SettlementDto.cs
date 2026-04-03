using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Api.Dto.Settlements;

public class SettlementDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Thumbnail { get; init; }
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = string.Empty;
    public int CreatedBy { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public bool IsOpen { get; init; }

    public SettlementDto() { }

    public SettlementDto(
        int id,
        string name,
        string? thumbnail,
        string currency,
        int createdBy,
        DateTime createdAtUtc,
        bool isOpen)
    {
        Id = id;
        Name = name;
        Thumbnail = thumbnail;
        Currency = currency;
        CreatedBy = createdBy;
        CreatedAtUtc = createdAtUtc;
        IsOpen = isOpen;
    }
}