using System.ComponentModel.DataAnnotations;

namespace WeShareClone.Api.Dto.Settlements;

public class AddSettlementUserDto
{
    [Range(1, int.MaxValue)]
    public int UserId { get; init; }

    public AddSettlementUserDto() { }

    public AddSettlementUserDto(int userId)
    {
        UserId = userId;
    }
}