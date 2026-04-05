using WeShareClone.Domain.Models;
using WeShareClone.Dto.Entries;
using WeShareClone.Dto.Settlements;
using WeShareClone.Dto.Users;

namespace WeShareClone.Extensions;

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
                createdAtUtc: settlement.CreatedAtUtc,
                isOpen: settlement.IsOpen
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
                addedAtUtc: entry.AddedAtUtc,
                distributions: entry.Distributions.Select(d => d.ToDto()).ToArray(),
                distributionMode: entry.DistributionMode.ToString()
            );
    }

    extension(EntryDistribution distribution)
    {
        public EntryDistributionDto ToDto()
            => new(userId: distribution.UserId, factor: distribution.Factor);
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
