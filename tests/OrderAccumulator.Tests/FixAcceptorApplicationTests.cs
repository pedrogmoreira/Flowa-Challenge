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
        var order = new NewOrderSingle(
            new ClOrdID(Guid.NewGuid().ToString()),
            new Symbol("PETR4"),
            new Side(Side.BUY),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );
        order.Set(new OrderQty(100));
        order.Set(new Price(10.50m));

        _exposureServiceMock
            .Setup(x => x.Apply("PETR4", Side.BUY, 10.50m, 100m))
            .Verifiable();

        // OnMessage will throw on SendToTarget since there's no active session
        // but we can verify Apply was called before that
        try { _sut.OnMessage(order, sessionID); } catch { }

        _exposureServiceMock.Verify(x => x.Apply("PETR4", Side.BUY, 10.50m, 100m), Times.Once);
    }
}