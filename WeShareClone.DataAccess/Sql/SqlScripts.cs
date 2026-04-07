using System.Reflection;

namespace WeShareClone.DataAccess.Sql;

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
    public static string GetMySettlements         => Load(nameof(GetMySettlements));
    public static string CreateSettlement         => Load(nameof(CreateSettlement));
    public static string UpdateSettlement         => Load(nameof(UpdateSettlement));
    public static string UpdateSettlementStatus   => Load(nameof(UpdateSettlementStatus));
    public static string DeleteSettlement         => Load(nameof(DeleteSettlement));
    public static string AddUserToSettlement      => Load(nameof(AddUserToSettlement));
    public static string RemoveUserFromSettlement => Load(nameof(RemoveUserFromSettlement));
    public static string IsSettlementParticipant  => Load(nameof(IsSettlementParticipant));
    public static string GetSettlementParticipantIds => Load(nameof(GetSettlementParticipantIds));
    public static string GetSettlementParticipants   => Load(nameof(GetSettlementParticipants));

    public static string GetSettlementDebtsBySettlementId   => Load(nameof(GetSettlementDebtsBySettlementId));
    public static string CreateSettlementDebt                => Load(nameof(CreateSettlementDebt));
    public static string UpdateSettlementDebtPaid            => Load(nameof(UpdateSettlementDebtPaid));
    public static string DeleteSettlementDebtsBySettlementId => Load(nameof(DeleteSettlementDebtsBySettlementId));

    public static string GetEntriesBySettlementId => Load(nameof(GetEntriesBySettlementId));
    public static string GetEntryById             => Load(nameof(GetEntryById));
    public static string CreateEntry              => Load(nameof(CreateEntry));
    public static string UpdateEntry              => Load(nameof(UpdateEntry));
    public static string DeleteEntry              => Load(nameof(DeleteEntry));

    public static string GetEntryDistributionsByEntryId      => Load(nameof(GetEntryDistributionsByEntryId));
    public static string GetEntryDistributionsBySettlementId => Load(nameof(GetEntryDistributionsBySettlementId));
    public static string UpsertEntryDistributions            => Load(nameof(UpsertEntryDistributions));

    public static string GetUserByEmail                => Load(nameof(GetUserByEmail));
    public static string GetUserById                   => Load(nameof(GetUserById));
    public static string CreateUser                    => Load(nameof(CreateUser));
    public static string UpdateUserName                => Load(nameof(UpdateUserName));
    public static string UpdateUserProfile             => Load(nameof(UpdateUserProfile));
    public static string SearchUsers                   => Load(nameof(SearchUsers));

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
    public static string UpdatePasskeyName                   => Load(nameof(UpdatePasskeyName));
    public static string DeletePasskeyCredentialById         => Load(nameof(DeletePasskeyCredentialById));
    public static string GetPasskeyChallengeByEmailAndType   => Load(nameof(GetPasskeyChallengeByEmailAndType));
    public static string UpsertPasskeyChallenge              => Load(nameof(UpsertPasskeyChallenge));
    public static string DeletePasskeyChallengeByEmailAndType => Load(nameof(DeletePasskeyChallengeByEmailAndType));

    public static string GetLatestExchangeRates      => Load(nameof(GetLatestExchangeRates));
    public static string InsertExchangeRateIfChanged => Load(nameof(InsertExchangeRateIfChanged));

    private static string Load(string key)
    {
        string resourceName = _scripts[key];
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
