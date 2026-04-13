using Microsoft.AspNetCore.Mvc;
using OrderGenerator.Api.Application;
using OrderGenerator.Api.Models;

namespace OrderGenerator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IFixInitiatorService fixInitiatorService, ILogger<OrdersController> logger) : ControllerBase
{

    /// <summary>
    /// Submits a new order to the Accumulator via FIX 4.4 and returns the execution result.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OrderRequest request)
    {
        logger.LogInformation(
            "Order received: {Symbol} {Side} {Qty}@{Price}",
            request.Symbol,
            request.Side,
            request.Quantity,
            request.Price
        );

        var response = await fixInitiatorService.SendOrderAsync(request, TimeSpan.FromSeconds(10));
        return Ok(response);
    }
}