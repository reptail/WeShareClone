namespace WeShareClone.Web.Models;

public class ExchangeRateModel
{
    public string Currency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateOnly Date { get; set; }
}
