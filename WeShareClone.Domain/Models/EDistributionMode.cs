namespace WeShareClone.Domain.Models;

/// <summary>Defines how the cost of an entry is distributed among participants.</summary>
public enum EDistributionMode : byte
{
    /// <summary>Each participant is assigned an equal share. All distribution factors must be identical.</summary>
    EvenSplit = 0,

    /// <summary>Each participant is assigned a percentage share. Distribution factors must sum to 1.0.</summary>
    Percentage = 1,

    /// <summary>Each participant is assigned a fixed monetary amount. Distribution factors must sum to the entry value.</summary>
    FixedAmount = 2,
}
