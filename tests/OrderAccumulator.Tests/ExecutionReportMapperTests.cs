using OrderAccumulator.Mappers;
using QuickFix.FIX44;
using QuickFix.Fields;

namespace OrderAccumulator.Tests;

public class ExecutionReportMapperTests
{
    [Fact]
    public void ToFill_SetsExecTypeAsFill()
    {
        var order = BuildOrder();
        var report = ExecutionReportMapper.ToFill(order);
        Assert.Equal(ExecType.FILL, report.ExecType.Value);
    }

    [Fact]
    public void ToFill_SetsOrdStatusAsFilled()
    {
        var order = BuildOrder();
        var report = ExecutionReportMapper.ToFill(order);
        Assert.Equal(OrdStatus.FILLED, report.OrdStatus.Value);
    }

    [Fact]
    public void ToFill_SetsLeavesQtyAsZero()
    {
        var order = BuildOrder();
        var report = ExecutionReportMapper.ToFill(order);
        Assert.Equal(0m, report.LeavesQty.Value);
    }

    [Fact]
    public void ToFill_SetsCumQtyFromOrder()
    {
        var order = BuildOrder();
        var report = ExecutionReportMapper.ToFill(order);
        Assert.Equal(100m, report.CumQty.Value);
    }

    [Fact]
    public void ToFill_SetsAvgPxFromOrder()
    {
        var order = BuildOrder();
        var report = ExecutionReportMapper.ToFill(order);
        Assert.Equal(10.50m, report.AvgPx.Value);
    }

    [Fact]
    public void ToFill_SetsClOrdIDFromOrder()
    {
        var order = BuildOrder();
        var report = ExecutionReportMapper.ToFill(order);
        Assert.Equal(order.ClOrdID.Value, report.ClOrdID.Value);
    }

    [Fact]
    public void ToFill_GeneratesUniqueOrderIDs()
    {
        var order = BuildOrder();
        var report1 = ExecutionReportMapper.ToFill(order);
        var report2 = ExecutionReportMapper.ToFill(order);
        Assert.NotEqual(report1.OrderID.Value, report2.OrderID.Value);
    }

    private static NewOrderSingle BuildOrder()
    {
        var order = new NewOrderSingle(
            new ClOrdID(Guid.NewGuid().ToString()),
            new Symbol("PETR4"),
            new Side(Side.BUY),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );
        order.Set(new OrderQty(100));
        order.Set(new Price(10.50m));
        return order;
    }
}