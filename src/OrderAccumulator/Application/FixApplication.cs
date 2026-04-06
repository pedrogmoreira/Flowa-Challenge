using OrderAccumulator.Mappers;
using QuickFix;
using QuickFix.FIX44;

namespace OrderAccumulator.Application;

public class FixApplication : MessageCracker, IApplication
{
    private readonly ExposureService _exposureService;
    private readonly ILogger<FixApplication> _logger;

    public FixApplication(ExposureService exposureService, ILogger<FixApplication> logger)
    {
        _exposureService = exposureService;
        _logger = logger;
    }

    public void FromApp(QuickFix.Message msg, SessionID sessionID)
        => Crack(msg, sessionID);

    public void OnCreate(SessionID sessionID)
        => _logger.LogInformation("Session created: {SessionID}", sessionID);

    public void OnLogon(SessionID sessionID)
        => _logger.LogInformation("Logon: {SessionID}", sessionID);

    public void OnLogout(SessionID sessionID)
        => _logger.LogInformation("Logout: {SessionID}", sessionID);

    public void FromAdmin(QuickFix.Message msg, SessionID sessionID) { }
    public void ToAdmin(QuickFix.Message msg, SessionID sessionID) { }
    public void ToApp(QuickFix.Message msg, SessionID sessionID) { }

    public void OnMessage(NewOrderSingle order, SessionID sessionID)
    {
        var symbol = order.Symbol.Value;
        var side = order.Side.Value;
        var price = order.Price.Value;
        var quantity = order.OrderQty.Value;

        _exposureService.Apply(symbol, side, (decimal)price, (decimal)quantity);

        SendExecutionReport(order, sessionID);
    }

    private void SendExecutionReport(NewOrderSingle order, SessionID sessionID)
    {
        var report = ExecutionReportMapper.ToFill(order);
        Session.SendToTarget(report, sessionID);

        _logger.LogInformation(
            "ExecutionReport sent: {ClOrdID} {Symbol} {Side} {Qty}@{Price}",
            order.ClOrdID.Value,
            order.Symbol.Value,
            order.Side.Value,
            order.OrderQty.Value,
            order.Price.Value
        );
    }
}