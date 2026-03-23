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
}