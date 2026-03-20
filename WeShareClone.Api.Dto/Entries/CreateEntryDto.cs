using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Api.Dto.Entries;

public class CreateEntryDto
{
    [Required]
    public string Name { get; init; } = string.Empty;
    [Range(0.01, double.MaxValue)]
    public decimal Value { get; init; }
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int AddedBy { get; init; }

    public CreateEntryDto() { }

    public CreateEntryDto(
        string name,
        decimal value,
        string currency,
        int addedBy)
    {
        Name = name;
        Value = value;
        Currency = currency;
        AddedBy = addedBy;
    }
}