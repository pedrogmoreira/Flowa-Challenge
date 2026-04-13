using System.ComponentModel.DataAnnotations;

namespace OrderGenerator.Api.Models;

public class OrderRequest
{
    [Required]
    [RegularExpression("^(PETR4|VALE3|VIIA4)$", ErrorMessage = "Symbol must be PETR4, VALE3 or VIIA4.")]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Buy|Sell)$", ErrorMessage = "Side must be Buy or Sell.")]
    public string Side { get; set; } = string.Empty;

    [Required]
    [Range(1, 99999, ErrorMessage = "Quantity must be a positive integer less than 100,000.")]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, 999.99, ErrorMessage = "Price must be between 0.01 and 999.99.")]
    public decimal Price { get; set; }
}