namespace WeShareClone.Api.Domain.Models;

public record PasskeyCredential(
    int Id,
    int UserId,
    byte[] CredentialId,
    byte[] PublicKey,
    long SignCount,
    Guid AaGuid,
    DateTime CreatedAtUtc
);