using OrderAccumulator.Mappers;
using QuickFix;
using QuickFix.FIX44;

namespace OrderAccumulator.Application;

/// <summary>
/// Implements the QuickFIX/n IApplication interface.
/// Entry point for all FIX messages received by the Accumulator.
/// Inherits MessageCracker for automatic dispatch by message type.
/// </summary>
public class FixApplication(ExposureService exposureService, ILogger<FixApplication> logger) : MessageCracker, IApplication
{
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

    /// <summary>
    /// Invoked by MessageCracker when a NewOrderSingle (35=D) is received.
    /// Extracts order fields, updates exposure, and replies with an ExecutionReport.
    /// </summary>
    public void OnMessage(NewOrderSingle order, SessionID sessionID)
    {
        var symbol = order.Symbol.Value;
        var side = order.Side.Value;
        var price = order.Price.Value;
        var quantity = order.OrderQty.Value;

        exposureService.Apply(symbol, side, (decimal)price, (decimal)quantity);

        SendExecutionReport(order, sessionID);
    }

    /// <summary>
    /// Builds and sends a FIX ExecutionReport (35=8) back to the Generator
    /// as a full fill response to the received NewOrderSingle.
    /// Uses ClOrdID for request/response correlation on the Generator side.
    /// </summary>
    private void SendExecutionReport(NewOrderSingle order, SessionID sessionID)
    {
        var report = ExecutionReportMapper.ToFill(order);
        Session.SendToTarget(report, sessionID);

        logger.LogInformation(
            "ExecutionReport sent: {ClOrdID} {Symbol} {Side} {Qty}@{Price}",
            order.ClOrdID.Value,
            order.Symbol.Value,
            order.Side.Value,
            order.OrderQty.Value,
            order.Price.Value
        );
    }
}