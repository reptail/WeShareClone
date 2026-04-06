using WeShareClone.DataAccess.Models;
using WeShareClone.Domain.Models;

namespace WeShareClone.DataAccess.Extensions;

public static class DbToDomainMappingExtensions
{
    extension(DbUser db)
    {
        public User ToDomain()
            => new(
                Id: db.Id,
                Email: db.Email,
                Name: db.Name,
                Role: db.Role,
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
                CreatedAtUtc: db.CreatedAtUtc,
                IsOpen: db.IsOpen
            );
    }

    extension(DbEntry db)
    {
        public Entry ToDomain(IReadOnlyList<EntryDistribution> distributions)
            => new(
                Id: db.Id,
                SettlementId: db.SettlementId,
                Name: db.Name,
                Value: db.Value,
                Currency: db.Currency,
                AddedBy: db.AddedBy,
                AddedAtUtc: db.AddedAtUtc,
                Distributions: distributions,
                DistributionMode: db.DistributionMode
            );
    }

    extension(DbEntryDistribution db)
    {
        public EntryDistribution ToDomain()
            => new(UserId: db.UserId, Factor: db.Factor);
    }

    extension(DbVerificationCode db)
    {
        public VerificationCode ToDomain()
            => new(
                Id: db.Id,
                Email: db.Email,
                CodeHash: db.CodeHash,
                CreatedAtUtc: db.CreatedAtUtc,
                ExpiresAtUtc: db.ExpiresAtUtc
            );
    }

    extension(DbRefreshToken db)
    {
        public RefreshToken ToDomain()
            => new(
                Id: db.Id,
                UserId: db.UserId,
                Token: db.Token,
                CreatedAtUtc: db.CreatedAtUtc,
                ExpiresAtUtc: db.ExpiresAtUtc
            );
    }

    extension(DbPendingSignup db)
    {
        public PendingSignup ToDomain()
            => new(
                Id: db.Id,
                Email: db.Email,
                Name: db.Name,
                CreatedAtUtc: db.CreatedAtUtc
            );
    }

    extension(DbPasskeyCredential db)
    {
        public PasskeyCredential ToDomain()
            => new(
                Id: db.Id,
                UserId: db.UserId,
                CredentialId: db.CredentialId,
                PublicKey: db.PublicKey,
                SignCount: db.SignCount,
                AaGuid: db.AaGuid,
                CreatedAtUtc: db.CreatedAtUtc
            );
    }

    extension(DbPasskeyChallenge db)
    {
        public PasskeyChallenge ToDomain()
            => new(
                Id: db.Id,
                Email: db.Email,
                ChallengeType: db.ChallengeType,
                OptionsJson: db.OptionsJson,
                ExpiresAtUtc: db.ExpiresAtUtc
            );
    }

    extension(DbExchangeRate db)
    {
        public ExchangeRate ToDomain()
            => new(
                Date: db.Date,
                Currency: db.Currency,
                Rate: db.Rate,
                ValidFromUtc: db.ValidFromUtc,
                ValidToUtc: db.ValidToUtc
            );
    }
}
