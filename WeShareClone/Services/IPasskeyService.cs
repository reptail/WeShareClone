using Fido2NetLib;
using WeShareClone.Domain.Models;

namespace WeShareClone.Services;

public interface IPasskeyService
{
    /// <summary>Generates credential creation options for a passkey registration. Requires an authenticated user.</summary>
    Task<CredentialCreateOptions> BeginRegistrationAsync(int userId, string email, string name);

    /// <summary>Validates and stores a new passkey credential. Requires an authenticated user. Returns the created credential.</summary>
    Task<PasskeyCredential> CompleteRegistrationAsync(int userId, string email, AuthenticatorAttestationRawResponse attestationResponse);

    /// <summary>Begins passkey login. Returns null if no passkeys are registered for this email.</summary>
    Task<AssertionOptions?> BeginLoginAsync(string email);

    /// <summary>Validates the passkey assertion and returns an AuthToken, or null if invalid.</summary>
    Task<AuthToken?> CompleteLoginAsync(AuthenticatorAssertionRawResponse assertionResponse);

    /// <summary>Returns all passkey credentials registered for the given user.</summary>
    Task<PasskeyCredential[]> GetCredentialsByUserIdAsync(int userId);

    /// <summary>Updates the name of the specified passkey. Returns the updated credential, or null if not found.</summary>
    Task<PasskeyCredential?> UpdateCredentialNameAsync(int userId, int credentialId, string? name);

    /// <summary>Deletes the specified passkey credential. Verifies ownership by userId.</summary>
    Task DeleteCredentialAsync(int userId, int credentialId);
}