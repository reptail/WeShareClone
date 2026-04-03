namespace WeShareClone.Api.DataAccess.Models;

public class DbSettlement
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Thumbnail { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int CreatedBy { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public bool IsOpen { get; init; }
}
