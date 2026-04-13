using OrderGenerator.Api.Mappers;
using OrderGenerator.Api.Models;
using QuickFix.Fields;

namespace OrderGenerator.Api.Tests;

public class NewOrderSingleMapperTests
{
    [Fact]
    public void ToNewOrderSingle_BuyOrder_SetsSideCorrectly()
    {
        var request = new OrderRequest
        {
            Symbol = "PETR4",
            Side = "Buy",
            Quantity = 100,
            Price = 10.50m
        };

        var (message, _) = NewOrderSingleMapper.ToNewOrderSingle(request);

        Assert.Equal(Side.BUY, message.Side.Value);
    }

    [Fact]
    public void ToNewOrderSingle_SellOrder_SetsSideCorrectly()
    {
        var request = new OrderRequest
        {
            Symbol = "VALE3",
            Side = "Sell",
            Quantity = 50,
            Price = 20.00m
        };

        var (message, _) = NewOrderSingleMapper.ToNewOrderSingle(request);

        Assert.Equal(Side.SELL, message.Side.Value);
    }

    [Fact]
    public void ToNewOrderSingle_SetsSymbolCorrectly()
    {
        var request = new OrderRequest
        {
            Symbol = "VIIA4",
            Side = "Buy",
            Quantity = 100,
            Price = 10.50m
        };

        var (message, _) = NewOrderSingleMapper.ToNewOrderSingle(request);

        Assert.Equal("VIIA4", message.Symbol.Value);
    }

    [Fact]
    public void ToNewOrderSingle_GeneratesUniqueClOrdIDs()
    {
        var request = new OrderRequest
        {
            Symbol = "PETR4",
            Side = "Buy",
            Quantity = 100,
            Price = 10.50m
        };

        var (_, clOrdID1) = NewOrderSingleMapper.ToNewOrderSingle(request);
        var (_, clOrdID2) = NewOrderSingleMapper.ToNewOrderSingle(request);

        Assert.NotEqual(clOrdID1, clOrdID2);
    }

    [Fact]
    public void ToNewOrderSingle_SetsQuantityAndPriceCorrectly()
    {
        var request = new OrderRequest
        {
            Symbol = "PETR4",
            Side = "Buy",
            Quantity = 250,
            Price = 33.75m
        };

        var (message, _) = NewOrderSingleMapper.ToNewOrderSingle(request);

        decimal expectedQty = 250m;
        decimal expectedPrice = 33.75m;
        decimal actualQty = message.OrderQty.Value;
        decimal actualPrice = message.Price.Value;

        Assert.Equal(expectedQty, actualQty);
        Assert.Equal(expectedPrice, actualPrice);
    }
}