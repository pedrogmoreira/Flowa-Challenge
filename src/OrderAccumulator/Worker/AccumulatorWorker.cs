using OrderAccumulator.Infrastructure;

namespace OrderAccumulator.Worker;

public class AccumulatorWorker(
    ISocketAcceptorWrapper acceptor,
    ILogger<AccumulatorWorker> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        acceptor.Start();
        logger.LogInformation("FIX Acceptor started");

        stoppingToken.Register(() =>
        {
            acceptor.Stop();
            logger.LogInformation("FIX Acceptor stopped");
        });

        return Task.CompletedTask;
    }
}