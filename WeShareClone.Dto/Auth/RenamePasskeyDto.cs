namespace WeShareClone.Dto.Auth;

/// <summary>Request body for renaming a passkey credential.</summary>
public class RenamePasskeyDto
{
    /// <summary>The new name for the passkey. Set to <c>null</c> to clear the name.</summary>
    public string? Name { get; init; }

    /// <summary>Initializes a new empty instance of <see cref="RenamePasskeyDto"/>.</summary>
    public RenamePasskeyDto() { }

    /// <summary>Initializes a new instance of <see cref="RenamePasskeyDto"/>.</summary>
    /// <param name="name">The new name.</param>
    public RenamePasskeyDto(string? name)
    {
        Name = name;
    }
}
