namespace OrderGenerator.Api.Models;

public class OrderResponse
{
    public string ClOrdID { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Side { get; set; } = string.Empty;
    public decimal ExecutedQty { get; set; }
    public decimal AvgPrice { get; set; }
    public string Status { get; set; } = string.Empty;
}