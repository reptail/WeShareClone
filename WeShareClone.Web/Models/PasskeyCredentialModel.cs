namespace WeShareClone.Web.Models;

public class PasskeyCredentialModel
{
    public int Id { get; init; }
    public Guid AaGuid { get; init; }
    public string? Name { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
