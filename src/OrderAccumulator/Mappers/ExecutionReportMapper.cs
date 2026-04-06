using QuickFix.FIX44;
using QuickFix.Fields;

namespace OrderAccumulator.Mappers;

public static class ExecutionReportMapper
{
    public static ExecutionReport ToFill(NewOrderSingle order)
    {
        var report = new ExecutionReport(
            new OrderID(Guid.NewGuid().ToString()),
            new ExecID(Guid.NewGuid().ToString()),
            new ExecType(ExecType.FILL),
            new OrdStatus(OrdStatus.FILLED),
            order.Symbol,
            order.Side,
            new LeavesQty(0),
            new CumQty(order.OrderQty.Value),
            new AvgPx(order.Price.Value)
        );

        report.Set(order.ClOrdID);
        report.Set(order.Symbol);
        report.Set(order.OrderQty);
        report.Set(order.Price);

        return report;
    }
}