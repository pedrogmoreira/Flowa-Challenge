using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrderGenerator.Api.Application;
using OrderGenerator.Api.Controllers;
using OrderGenerator.Api.Models;

namespace OrderGenerator.Api.Tests;

public class OrdersControllerTests
{
    private readonly Mock<IFixInitiatorService> _fixServiceMock;
    private readonly OrdersController _sut;

    public OrdersControllerTests()
    {
        _fixServiceMock = new Mock<IFixInitiatorService>();
        _sut = new OrdersController(_fixServiceMock.Object, new Mock<ILogger<OrdersController>>().Object);
    }

    [Fact]
    public async Task Post_ValidOrder_ReturnsOkWithFilledStatus()
    {
        var request = new OrderRequest
        {
            Symbol = "PETR4",
            Side = "Buy",
            Quantity = 100,
            Price = 10.50m
        };

        var expectedResponse = new OrderResponse
        {
            ClOrdID = Guid.NewGuid().ToString(),
            Symbol = "PETR4",
            Side = "Buy",
            ExecutedQty = 100,
            AvgPrice = 10.50m,
            Status = "Filled"
        };

        _fixServiceMock
            .Setup(x => x.SendOrderAsync(request, It.IsAny<TimeSpan>()))
            .ReturnsAsync(expectedResponse);

        var result = await _sut.Post(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<OrderResponse>(okResult.Value);
        Assert.Equal("Filled", response.Status);
        Assert.Equal("PETR4", response.Symbol);
    }

    [Fact]
    public async Task Post_ValidSellOrder_ReturnsOkWithFilledStatus()
    {
        var request = new OrderRequest
        {
            Symbol = "VALE3",
            Side = "Sell",
            Quantity = 50,
            Price = 20.00m
        };

        var expectedResponse = new OrderResponse
        {
            ClOrdID = Guid.NewGuid().ToString(),
            Symbol = "VALE3",
            Side = "Sell",
            ExecutedQty = 50,
            AvgPrice = 20.00m,
            Status = "Filled"
        };

        _fixServiceMock
            .Setup(x => x.SendOrderAsync(request, It.IsAny<TimeSpan>()))
            .ReturnsAsync(expectedResponse);

        var result = await _sut.Post(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<OrderResponse>(okResult.Value);
        Assert.Equal("Filled", response.Status);
        Assert.Equal("VALE3", response.Symbol);
    }
}