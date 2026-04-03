using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Api.Dto.Settlements;

public class UpdateSettlementDto
{
    [Required]
    public string Name { get; init; } = string.Empty;
    public string? Thumbnail { get; init; }
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = string.Empty;
    public bool IsOpen { get; init; } = true;

    public UpdateSettlementDto() { }

    public UpdateSettlementDto(
        string name,
        string? thumbnail,
        string currency,
        bool isOpen = true)
    {
        Name = name;
        Thumbnail = thumbnail;
        Currency = currency;
        IsOpen = isOpen;
    }
}