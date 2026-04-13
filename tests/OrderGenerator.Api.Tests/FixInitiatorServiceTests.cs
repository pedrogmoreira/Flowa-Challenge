using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OrderGenerator.Api.Application;
using OrderGenerator.Api.Infrastructure;
using OrderGenerator.Api.Models;
using QuickFix;
using QuickFix.FIX44;
using QuickFix.Fields;

namespace OrderGenerator.Api.Tests;

public class FixInitiatorServiceTests
{
    private readonly Mock<ISocketInitiatorWrapper> _wrapperMock;
    private readonly FixInitiatorService _sut;

    public FixInitiatorServiceTests()
    {
        _wrapperMock = new Mock<ISocketInitiatorWrapper>();
        _sut = new FixInitiatorService(
            _wrapperMock.Object,
            NullLogger<FixInitiatorService>.Instance
        );
    }

    [Fact]
    public async Task StartAsync_StartsInitiator()
    {
        await _sut.StartAsync(CancellationToken.None);

        _wrapperMock.Verify(x => x.Start(It.IsAny<IApplication>()), Times.Once);
    }

    [Fact]
    public async Task StopAsync_StopsInitiator()
    {
        await _sut.StopAsync(CancellationToken.None);

        _wrapperMock.Verify(x => x.Stop(), Times.Once);
    }

    [Fact]
    public void OnMessage_WithUnknownClOrdID_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "GENERATOR", "ACCUMULATOR");
        var report = BuildExecutionReport(Guid.NewGuid().ToString());

        var exception = Record.Exception(() => _sut.OnMessage(report, sessionID));

        Assert.Null(exception);
    }

    [Fact]
    public async Task OnMessage_WithKnownClOrdID_SetsResult()
    {
        var clOrdID = Guid.NewGuid().ToString();
        var sessionID = new SessionID("FIX.4.4", "GENERATOR", "ACCUMULATOR");
        var tcs = new TaskCompletionSource<OrderResponse>();
        _sut._pending[clOrdID] = tcs;

        var report = BuildExecutionReport(clOrdID);
        _sut.OnMessage(report, sessionID);

        var response = await tcs.Task;

        Assert.Equal(clOrdID, response.ClOrdID);
        Assert.Equal("PETR4", response.Symbol);
        Assert.Equal("Buy", response.Side);
        Assert.Equal(100m, response.ExecutedQty);
        Assert.Equal(10.50m, response.AvgPrice);
        Assert.Equal("Filled", response.Status);
    }

    [Fact]
    public void FromApp_WithExecutionReport_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "GENERATOR", "ACCUMULATOR");
        var report = BuildExecutionReport(Guid.NewGuid().ToString());

        var exception = Record.Exception(() => _sut.FromApp(report, sessionID));

        Assert.Null(exception);
    }

    [Fact]
    public void OnCreate_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "GENERATOR", "ACCUMULATOR");
        var exception = Record.Exception(() => _sut.OnCreate(sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void OnLogon_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "GENERATOR", "ACCUMULATOR");
        var exception = Record.Exception(() => _sut.OnLogon(sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void OnLogout_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "GENERATOR", "ACCUMULATOR");
        var exception = Record.Exception(() => _sut.OnLogout(sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void FromAdmin_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "GENERATOR", "ACCUMULATOR");
        var exception = Record.Exception(() => _sut.FromAdmin(new QuickFix.Message(), sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void ToAdmin_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "GENERATOR", "ACCUMULATOR");
        var exception = Record.Exception(() => _sut.ToAdmin(new QuickFix.Message(), sessionID));
        Assert.Null(exception);
    }

    [Fact]
    public void ToApp_DoesNotThrow()
    {
        var sessionID = new SessionID("FIX.4.4", "GENERATOR", "ACCUMULATOR");
        var exception = Record.Exception(() => _sut.ToApp(new QuickFix.Message(), sessionID));
        Assert.Null(exception);
    }

    private static ExecutionReport BuildExecutionReport(string clOrdID)
    {
        var report = new ExecutionReport(
            new OrderID("ORD001"),
            new ExecID(Guid.NewGuid().ToString()),
            new ExecType(ExecType.FILL),
            new OrdStatus(OrdStatus.FILLED),
            new Symbol("PETR4"),
            new Side(Side.BUY),
            new LeavesQty(0),
            new CumQty(100),
            new AvgPx(10.50m)
        );
        report.Set(new ClOrdID(clOrdID));
        return report;
    }
}
