using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Api.Dto.Settlements;

public class CreateSettlementDto
{
    [Required]
    public string Name { get; init; } = string.Empty;
    public string? Thumbnail { get; init; }
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int CreatedBy { get; init; }

    public CreateSettlementDto() { }

    public CreateSettlementDto(
        string name,
        string? thumbnail,
        string currency,
        int createdBy)
    {
        Name = name;
        Thumbnail = thumbnail;
        Currency = currency;
        CreatedBy = createdBy;
    }
}