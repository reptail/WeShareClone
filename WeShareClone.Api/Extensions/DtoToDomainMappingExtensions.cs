using WeShareClone.Api.Domain.Models;
using WeShareClone.Api.Dto.Entries;
using WeShareClone.Api.Dto.Settlements;

namespace WeShareClone.Api.Extensions;

public static class DtoToDomainMappingExtensions
{
    extension(CreateSettlementDto dto)
    {
        public Settlement ToDomain()
            => new(
                Id: 0,
                Name: dto.Name,
                Thumbnail: dto.Thumbnail,
                Currency: dto.Currency,
                CreatedBy: dto.CreatedBy,
                CreatedAtUtc: default
            );
    }

    extension(UpdateSettlementDto dto)
    {
        public Settlement ToDomain(int id)
            => new(
                Id: id,
                Name: dto.Name,
                Thumbnail: dto.Thumbnail,
                Currency: dto.Currency,
                CreatedBy: 0,
                CreatedAtUtc: default
            );
    }

    extension(CreateEntryDto dto)
    {
        public Entry ToDomain(int settlementId)
            => new(
                Id: 0,
                SettlementId: settlementId,
                Name: dto.Name,
                Value: dto.Value,
                Currency: dto.Currency,
                AddedBy: dto.AddedBy,
                AddedAtUtc: default
            );
    }

    extension(UpdateEntryDto dto)
    {
        public Entry ToDomain(int id, int settlementId)
            => new(
                Id: id,
                SettlementId: settlementId,
                Name: dto.Name,
                Value: dto.Value,
                Currency: dto.Currency,
                AddedBy: 0,
                AddedAtUtc: default
            );
    }
}