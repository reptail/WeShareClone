using WeShareClone.Api.DataAccess.Models;
using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.DataAccess.Extensions;

public static class DomainToDbMappingExtensions
{
    extension(User domain)
    {
        public DbUser ToDb()
            => new()
            {
                Id = domain.Id,
                Email = domain.Email,
                Name = domain.Name,
                Role = domain.Role,
                JoinedAtUtc = domain.JoinedAtUtc,
                IsDeleted = domain.IsDeleted,
            };
    }

    extension(Settlement domain)
    {
        public DbSettlement ToDb()
            => new()
            {
                Id = domain.Id,
                Name = domain.Name,
                Thumbnail = domain.Thumbnail,
                Currency = domain.Currency,
                CreatedBy = domain.CreatedBy,
                CreatedAtUtc = domain.CreatedAtUtc,
            };
    }

    extension(Entry domain)
    {
        public DbEntry ToDb()
            => new()
            {
                Id = domain.Id,
                SettlementId = domain.SettlementId,
                Name = domain.Name,
                Value = domain.Value,
                Currency = domain.Currency,
                AddedBy = domain.AddedBy,
                AddedAtUtc = domain.AddedAtUtc,
            };
    }
}