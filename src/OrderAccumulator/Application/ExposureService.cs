using System.Collections.Concurrent;

namespace OrderAccumulator.Application;

public class ExposureService(ILogger<ExposureService> logger)
{
    private readonly ConcurrentDictionary<string, decimal> _exposure = new();
    private readonly ILogger<ExposureService> _logger = logger;

    public void Apply(string symbol, char side, decimal price, decimal quantity)
    {
        var notional = price * quantity;

        _exposure.AddOrUpdate(
            symbol,
            side == '1' ? notional : -notional,
            (_, current) => side == '1' ? current + notional : current - notional
        );

        _logger.LogInformation(
            "Exposure updated: {Symbol} = {Exposure}",
            symbol,
            _exposure[symbol]
        );
    }
}