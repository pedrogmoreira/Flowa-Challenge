using System.Collections.Concurrent;
using QuickFix;
using QuickFix.FIX44;
using QuickFix.Fields;
using OrderGenerator.Api.Infrastructure;
using OrderGenerator.Api.Models;
using OrderGenerator.Api.Mappers;

namespace OrderGenerator.Api.Application;

/// <summary>
/// Manages the FIX initiator lifecycle and handles request/response correlation
/// between HTTP requests and asynchronous FIX ExecutionReport callbacks.
/// </summary>
public class FixInitiatorService(ISocketInitiatorWrapper wrapper, ILogger<FixInitiatorService> logger)
    : MessageCracker, IApplication, IHostedService, IFixInitiatorService
{
    internal readonly ConcurrentDictionary<string, TaskCompletionSource<OrderResponse>> _pending = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        wrapper.Start(this);
        logger.LogInformation("FIX Initiator started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        wrapper.Stop();
        logger.LogInformation("FIX Initiator stopped");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends a NewOrderSingle to the Accumulator and awaits the ExecutionReport.
    /// Uses TaskCompletionSource to bridge the synchronous FIX send
    /// with the asynchronous ExecutionReport callback.
    /// </summary>
    public async Task<OrderResponse> SendOrderAsync(OrderRequest request, TimeSpan timeout)
    {
        var (message, clOrdID) = NewOrderSingleMapper.ToNewOrderSingle(request);
        var tcs = new TaskCompletionSource<OrderResponse>();

        _pending[clOrdID] = tcs;

        Session.SendToTarget(message, GetSessionID());

        logger.LogInformation(
            "NewOrderSingle sent: {ClOrdID} {Symbol} {Side} {Qty}@{Price}",
            clOrdID,
            request.Symbol,
            request.Side,
            request.Quantity,
            request.Price
        );

        // Awaits the ExecutionReport with a timeout to avoid hanging requests.
        var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout));

        if (completed != tcs.Task)
        {
            _pending.TryRemove(clOrdID, out _);
            throw new TimeoutException($"No ExecutionReport received for ClOrdID {clOrdID}");
        }

        return await tcs.Task;
    }

    /// <summary>
    /// Invoked by MessageCracker when an ExecutionReport (35=8) is received.
    /// Resolves the pending TaskCompletionSource for the matching ClOrdID.
    /// </summary>
    public void OnMessage(ExecutionReport report, SessionID sessionID)
    {
        var clOrdID = report.ClOrdID.Value;

        if (!_pending.TryRemove(clOrdID, out var tcs))
        {
            logger.LogWarning("Received ExecutionReport for unknown ClOrdID: {ClOrdID}", clOrdID);
            return;
        }

        var response = new OrderResponse
        {
            ClOrdID = clOrdID,
            Symbol = report.Symbol.Value,
            Side = report.Side.Value == Side.BUY ? "Buy" : "Sell",
            ExecutedQty = (decimal)report.CumQty.Value,
            AvgPrice = (decimal)report.AvgPx.Value,
            Status = report.OrdStatus.Value == OrdStatus.FILLED ? "Filled" : "Unknown"
        };

        tcs.SetResult(response);
    }

    public void FromApp(QuickFix.Message msg, SessionID sessionID)
        => Crack(msg, sessionID);

    public void OnCreate(SessionID sessionID)
        => logger.LogInformation("Session created: {SessionID}", sessionID);

    public void OnLogon(SessionID sessionID)
        => logger.LogInformation("Logon: {SessionID}", sessionID);

    public void OnLogout(SessionID sessionID)
        => logger.LogInformation("Logout: {SessionID}", sessionID);

    public void FromAdmin(QuickFix.Message msg, SessionID sessionID) { }
    public void ToAdmin(QuickFix.Message msg, SessionID sessionID) { }
    public void ToApp(QuickFix.Message msg, SessionID sessionID) { }

    private SessionID GetSessionID() => wrapper.GetSessionIDs().First();
}
