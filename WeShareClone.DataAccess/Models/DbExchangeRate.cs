namespace WeShareClone.DataAccess.Models;

public class DbExchangeRate
{
    public DateOnly Date { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal Rate { get; init; }
    public DateTime ValidFromUtc { get; init; }
    public DateTime ValidToUtc { get; init; }
}
