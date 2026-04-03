namespace WeShareClone.Api.Dto;

/// <summary>Constant string values for the valid distribution modes used in entry DTOs.</summary>
public static class DistributionModes
{
    /// <summary>Each participant receives an equal share. All distribution factors must be identical.</summary>
    public const string EvenSplit = "EvenSplit";

    /// <summary>Each participant receives a percentage share. Distribution factors must sum to 1.0.</summary>
    public const string Percentage = "Percentage";

    /// <summary>Each participant receives a fixed monetary amount. Distribution factors must sum to the entry value.</summary>
    public const string FixedAmount = "FixedAmount";

    /// <summary>A regex pattern matching any valid distribution mode value.</summary>
    public const string ValidationPattern = $"^({EvenSplit}|{Percentage}|{FixedAmount})$";
}
