using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Api.Dto.Entries;

public class UpdateEntryDto
{
    [Required]
    public string Name { get; init; } = string.Empty;
    [Range(0.01, double.MaxValue)]
    public decimal Value { get; init; }
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = string.Empty;
    [Required]
    [MinLength(1)]
    public EntryDistributionDto[] Distributions { get; init; } = [];

    public UpdateEntryDto() { }

    public UpdateEntryDto(
        string name,
        decimal value,
        string currency,
        EntryDistributionDto[] distributions)
    {
        Name = name;
        Value = value;
        Currency = currency;
        Distributions = distributions;
    }
}
