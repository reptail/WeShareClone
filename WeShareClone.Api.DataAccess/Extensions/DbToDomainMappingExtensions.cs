using WeShareClone.Api.DataAccess.Models;
using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.DataAccess.Extensions;

public static class DbToDomainMappingExtensions
{
    extension(DbUser db)
    {
        public User ToDomain()
            => new(
                Id: db.Id,
                Email: db.Email,
                Name: db.Name,
                Thumbnail: db.Thumbnail,
                JoinedAtUtc: db.JoinedAtUtc,
                IsDeleted: db.IsDeleted
            );
    }

    extension(DbSettlement db)
    {
        public Settlement ToDomain()
            => new(
                Id: db.Id,
                Name: db.Name,
                Thumbnail: db.Thumbnail,
                Currency: db.Currency,
                CreatedBy: db.CreatedBy,
                CreatedAtUtc: db.CreatedAtUtc
            );
    }

    extension(DbEntry db)
    {
        public Entry ToDomain()
            => new(
                Id: db.Id,
                SettlementId: db.SettlementId,
                Name: db.Name,
                Value: db.Value,
                Currency: db.Currency,
                AddedBy: db.AddedBy,
                AddedAtUtc: db.AddedAtUtc
            );
    }
}