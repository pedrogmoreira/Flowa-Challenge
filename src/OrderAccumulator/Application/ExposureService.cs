using System.Collections.Concurrent;

namespace OrderAccumulator.Application;

/// <summary>
/// Maintains and updates the accumulated financial exposure per symbol.
/// Exposure = Σ(price × qty) buys - Σ(price × qty) sells.
/// </summary>
public class ExposureService(ILogger<ExposureService> logger)
{
    private readonly ConcurrentDictionary<string, decimal> _exposure = new();

    /// <summary>
    /// Applies the financial impact of an executed order to the symbol's exposure.
    /// </summary>
    /// <param name="symbol">Asset symbol (FIX tag 55)</param>
    /// <param name="side">Order side: '1' = Buy, '2' = Sell (FIX tag 54)</param>
    /// <param name="price">Execution price (FIX tag 44)</param>
    /// <param name="quantity">Executed quantity (FIX tag 38)</param>
    public void Apply(string symbol, char side, decimal price, decimal quantity)
    {
        var notional = price * quantity;

        _exposure.AddOrUpdate(
            symbol,
            side == '1' ? notional : -notional,
            (_, current) => side == '1' ? current + notional : current - notional
        );

        logger.LogInformation(
            "Exposure updated: {Symbol} = {Exposure}",
            symbol,
            _exposure[symbol]
        );
    }
}