using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;
using OrderAccumulator.Application;

namespace OrderAccumulator.Worker;

public class AccumulatorWorker(
    IConfiguration configuration,
    FixApplication fixApplication,
    ILogger<AccumulatorWorker> logger) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly FixApplication _fixApplication = fixApplication;
    private readonly ILogger<AccumulatorWorker> _logger = logger;
    private ThreadedSocketAcceptor? _acceptor;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = BuildSessionSettings();

        var storeFactory = new MemoryStoreFactory();
        var logFactory = new ScreenLogFactory(settings);

        _acceptor = new ThreadedSocketAcceptor(
            _fixApplication,
            storeFactory,
            settings,
            logFactory
        );

        _acceptor.Start();
        _logger.LogInformation("FIX Acceptor started on port {Port}",
            _configuration["QuickFix:SocketAcceptPort"]);

        stoppingToken.Register(() =>
        {
            _acceptor.Stop();
            _logger.LogInformation("FIX Acceptor stopped");
        });

        return Task.CompletedTask;
    }

    private SessionSettings BuildSessionSettings()
    {
        var path = _configuration["QuickFix:ConfigPath"] ?? "fix-configs/acceptor.cfg";
        return new SessionSettings(path);
    }
}