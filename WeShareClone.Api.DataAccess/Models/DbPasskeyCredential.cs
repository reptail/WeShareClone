namespace WeShareClone.Api.DataAccess.Models;

public class DbPasskeyCredential
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public byte[] CredentialId { get; init; } = [];
    public byte[] PublicKey { get; init; } = [];
    public long SignCount { get; init; }
    public Guid AaGuid { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}