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

    public UpdateSettlementDto() { }

    public UpdateSettlementDto(
        string name,
        string? thumbnail,
        string currency)
    {
        Name = name;
        Thumbnail = thumbnail;
        Currency = currency;
    }
}