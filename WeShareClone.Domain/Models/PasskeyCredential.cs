namespace WeShareClone.Domain.Models;

public record PasskeyCredential(
    int Id,
    int UserId,
    byte[] CredentialId,
    byte[] PublicKey,
    long SignCount,
    Guid AaGuid,
    string? Name,
    DateTime CreatedAtUtc
);