using System.Reflection;

namespace WeShareClone.Api.DataAccess.Sql;

public static class SqlScripts
{
    private static readonly Dictionary<string, string> _scripts;

    static SqlScripts()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        _scripts = assembly
            .GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                keySelector: name => name[..^4].Split('.').Last(),
                elementSelector: name => name
            );
    }

    public static string GetAllSettlements        => Load(nameof(GetAllSettlements));
    public static string GetSettlementById        => Load(nameof(GetSettlementById));
    public static string CreateSettlement         => Load(nameof(CreateSettlement));
    public static string UpdateSettlement         => Load(nameof(UpdateSettlement));
    public static string DeleteSettlement         => Load(nameof(DeleteSettlement));
    public static string AddUserToSettlement      => Load(nameof(AddUserToSettlement));
    public static string RemoveUserFromSettlement => Load(nameof(RemoveUserFromSettlement));

    public static string GetEntriesBySettlementId => Load(nameof(GetEntriesBySettlementId));
    public static string GetEntryById             => Load(nameof(GetEntryById));
    public static string CreateEntry              => Load(nameof(CreateEntry));
    public static string UpdateEntry              => Load(nameof(UpdateEntry));
    public static string DeleteEntry              => Load(nameof(DeleteEntry));

    public static string GetUserByEmail                => Load(nameof(GetUserByEmail));
    public static string GetUserById                   => Load(nameof(GetUserById));
    public static string CreateUser                    => Load(nameof(CreateUser));

    public static string UpsertVerificationCode        => Load(nameof(UpsertVerificationCode));
    public static string GetVerificationCodeByEmail    => Load(nameof(GetVerificationCodeByEmail));
    public static string DeleteVerificationCodeByEmail => Load(nameof(DeleteVerificationCodeByEmail));
    public static string CreateRefreshToken            => Load(nameof(CreateRefreshToken));
    public static string GetRefreshTokenByToken        => Load(nameof(GetRefreshTokenByToken));
    public static string DeleteRefreshTokenByToken     => Load(nameof(DeleteRefreshTokenByToken));

    public static string GetPendingSignupByEmail       => Load(nameof(GetPendingSignupByEmail));
    public static string UpsertPendingSignup           => Load(nameof(UpsertPendingSignup));
    public static string DeletePendingSignupByEmail    => Load(nameof(DeletePendingSignupByEmail));

    public static string GetPasskeyCredentialsByUserId       => Load(nameof(GetPasskeyCredentialsByUserId));
    public static string GetPasskeyCredentialByCredentialId  => Load(nameof(GetPasskeyCredentialByCredentialId));
    public static string CreatePasskeyCredential             => Load(nameof(CreatePasskeyCredential));
    public static string UpdatePasskeySignCount              => Load(nameof(UpdatePasskeySignCount));
    public static string DeletePasskeyCredentialById         => Load(nameof(DeletePasskeyCredentialById));
    public static string GetPasskeyChallengeByEmailAndType   => Load(nameof(GetPasskeyChallengeByEmailAndType));
    public static string UpsertPasskeyChallenge              => Load(nameof(UpsertPasskeyChallenge));
    public static string DeletePasskeyChallengeByEmailAndType => Load(nameof(DeletePasskeyChallengeByEmailAndType));

    private static string Load(string key)
    {
        string resourceName = _scripts[key];
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}