using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Dto.Entries;
using WeShareClone.Api.Dto.Settlements;
using WeShareClone.Api.Dto.Users;

namespace WeShareClone.Api.Extensions;

public static class DomainToDtoMappingExtensions
{
    extension(Settlement settlement)
    {
        public SettlementDto ToDto()
            => new(
                id: settlement.Id,
                name: settlement.Name,
                thumbnail: settlement.Thumbnail,
                currency: settlement.Currency,
                createdBy: settlement.CreatedBy,
                createdAtUtc: settlement.CreatedAtUtc
            );
    }

    extension(Entry entry)
    {
        public EntryDto ToDto()
            => new(
                id: entry.Id,
                settlementId: entry.SettlementId,
                name: entry.Name,
                value: entry.Value,
                currency: entry.Currency,
                addedBy: entry.AddedBy,
                addedAtUtc: entry.AddedAtUtc
            );
    }

    extension(User user)
    {
        public UserDto ToDto()
            => new(
                id: user.Id,
                email: user.Email,
                name: user.Name,
                role: user.Role.ToString()
            );
    }
}