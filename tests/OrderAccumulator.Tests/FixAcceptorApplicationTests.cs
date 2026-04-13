using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OrderAccumulator.Application;
using QuickFix;
using QuickFix.FIX44;
using QuickFix.Fields;

namespace OrderAccumulator.Tests;

public class FixAcceptorApplicationTests
{
    private readonly Mock<ExposureService> _exposureServiceMock;
    private readonly FixAcceptorApplication _sut;

    public FixAcceptorApplicationTests()
    {
        _exposureServiceMock = new Mock<ExposureService>(NullLogger<ExposureService>.Instance);
        _sut = new FixAcceptorApplication(_exposureServiceMock.Object, NullLogger<FixAcceptorApplication>.Instance);
    }

    [Fact]
    public void OnMessage_BuyOrder_CallsApplyWithCorrectParameters()
    {
        var sessionID = new SessionID("FIX.4.4", "ACCUMULATOR", "GENERATOR");
        var order = BuildOrder("PETR4", Side.BUY, 10.50m, 100m);

        try { _sut.OnMessage(order, sessionID); } catch { }

        _exposureServiceMock.Verify(x => x.Apply("PETR4", Side.BUY, 10.50m, 100m), Times.Once);
    }

    [Fact]
    public void OnMessage_SellOrder_CallsApplyWithCorrectParameters()
    {
        var sessionID = new SessionID("FIX.4.4", "ACCUMULATOR", "GENERATOR");
        var order = BuildOrder("VALE3", Side.SELL, 20.00m, 50m);

        try { _sut.OnMessage(order, sessionID); } catch { }

        _exposureServiceMock.Verify(x => x.Apply("VALE3", Side.SELL, 20.00m, 50m), Times.Once);
    }

    [Fact]
    public void OnCreate_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "ACCUMULATOR", "GENERATOR");
        var exception = Record.Exception(() => _sut.OnCreate(sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void OnLogon_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "ACCUMULATOR", "GENERATOR");
        var exception = Record.Exception(() => _sut.OnLogon(sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void OnLogout_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "ACCUMULATOR", "GENERATOR");
        var exception = Record.Exception(() => _sut.OnLogout(sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void FromAdmin_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "ACCUMULATOR", "GENERATOR");
        var exception = Record.Exception(() => _sut.FromAdmin(new QuickFix.Message(), sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void ToAdmin_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "ACCUMULATOR", "GENERATOR");
        var exception = Record.Exception(() => _sut.ToAdmin(new QuickFix.Message(), sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void ToApp_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "ACCUMULATOR", "GENERATOR");
        var exception = Record.Exception(() => _sut.ToApp(new QuickFix.Message(), sessionID));
        Assert.Null(exception);
    }

    private static NewOrderSingle BuildOrder(string symbol, char side, decimal price, decimal qty)
    {
        var order = new NewOrderSingle(
            new ClOrdID(Guid.NewGuid().ToString()),
            new Symbol(symbol),
            new Side(side),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );
        order.Set(new OrderQty(qty));
        order.Set(new Price(price));
        return order;
    }
}