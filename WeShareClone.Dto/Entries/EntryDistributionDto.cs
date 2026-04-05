using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Dto.Entries;

public class EntryDistributionDto
{
    public int UserId { get; init; }
    [Range(0.01, double.MaxValue)]
    public decimal Factor { get; init; }

    public EntryDistributionDto() { }

    public EntryDistributionDto(int userId, decimal factor)
    {
        UserId = userId;
        Factor = factor;
    }
}
