using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;
using OrderAccumulator.Application;

namespace OrderAccumulator.Worker;

public class AccumulatorWorker(
    IConfiguration configuration,
    FixAcceptorApplication fixApplication,
    ILogger<AccumulatorWorker> logger) : BackgroundService
{
    private ThreadedSocketAcceptor? _acceptor;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = BuildSessionSettings();

        var storeFactory = new MemoryStoreFactory();
        var logFactory = new ScreenLogFactory(settings);

        _acceptor = new ThreadedSocketAcceptor(
            fixApplication,
            storeFactory,
            settings,
            logFactory
        );

        _acceptor.Start();
        logger.LogInformation("FIX Acceptor started on port {Port}",
            configuration["QuickFix:SocketAcceptPort"]);

        stoppingToken.Register(() =>
        {
            _acceptor.Stop();
            logger.LogInformation("FIX Acceptor stopped");
        });

        return Task.CompletedTask;
    }

    private SessionSettings BuildSessionSettings()
    {
        var path = configuration["QuickFix:ConfigPath"] ?? "fix-configs/acceptor.cfg";
        return new SessionSettings(path);
    }
}