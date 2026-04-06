using QuickFix.FIX44;
using QuickFix.Fields;
using OrderGenerator.Api.Models;

namespace OrderGenerator.Api.Mappers;

/// <summary>
/// Builds a FIX 4.4 NewOrderSingle message from an OrderRequest.
/// </summary>
public static class NewOrderSingleMapper
{
    /// <summary>
    /// Maps an OrderRequest to a NewOrderSingle (35=D) FIX message.
    /// Generates a unique ClOrdID for request/response correlation.
    /// </summary>
    public static (NewOrderSingle Message, string ClOrdID) ToNewOrderSingle(OrderRequest request)
    {
        var clOrdID = Guid.NewGuid().ToString();

        var message = new NewOrderSingle(
            new ClOrdID(clOrdID),
            new Symbol(request.Symbol),
            new Side(request.Side == "Buy" ? Side.BUY : Side.SELL),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );

        message.Set(new OrderQty(request.Quantity));
        message.Set(new Price(request.Price));

        return (message, clOrdID);
    }
}