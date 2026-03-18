using System.Reflection;

namespace WeShareClone.Api.DataAccess.Sql;

/// <summary>
/// Provides access to embedded SQL scripts in the DataAccess assembly.
/// Scripts are resolved by filename (without extension) and loaded on demand.
/// </summary>
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

    public static string GetAllSettlements         => Load(nameof(GetAllSettlements));
    public static string GetSettlementById         => Load(nameof(GetSettlementById));
    public static string CreateSettlement          => Load(nameof(CreateSettlement));
    public static string UpdateSettlement          => Load(nameof(UpdateSettlement));
    public static string DeleteSettlement          => Load(nameof(DeleteSettlement));
    public static string AddUserToSettlement       => Load(nameof(AddUserToSettlement));
    public static string RemoveUserFromSettlement  => Load(nameof(RemoveUserFromSettlement));

    public static string GetEntriesBySettlementId  => Load(nameof(GetEntriesBySettlementId));
    public static string GetEntryById              => Load(nameof(GetEntryById));
    public static string CreateEntry               => Load(nameof(CreateEntry));
    public static string UpdateEntry               => Load(nameof(UpdateEntry));
    public static string DeleteEntry               => Load(nameof(DeleteEntry));

    private static string Load(string key)
    {
        string resourceName = _scripts[key];
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
