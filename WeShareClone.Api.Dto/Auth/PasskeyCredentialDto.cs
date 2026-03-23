namespace WeShareClone.Api.Dto.Auth;

/// <summary>Represents a registered passkey credential.</summary>
public class PasskeyCredentialDto
{
    /// <summary>The database identifier of the credential (use for deletion).</summary>
    public int Id { get; init; }

    /// <summary>The authenticator AAGUID, identifying the type of authenticator device.</summary>
    public Guid AaGuid { get; init; }

    /// <summary>The UTC date and time the passkey was registered.</summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>Initializes a new empty instance of <see cref="PasskeyCredentialDto"/>.</summary>
    public PasskeyCredentialDto() { }

    /// <summary>Initializes a new instance of <see cref="PasskeyCredentialDto"/>.</summary>
    /// <param name="id">The credential database identifier.</param>
    /// <param name="aaGuid">The authenticator AAGUID.</param>
    /// <param name="createdAtUtc">The registration timestamp.</param>
    public PasskeyCredentialDto(int id, Guid aaGuid, DateTime createdAtUtc)
    {
        Id = id;
        AaGuid = aaGuid;
        CreatedAtUtc = createdAtUtc;
    }
}