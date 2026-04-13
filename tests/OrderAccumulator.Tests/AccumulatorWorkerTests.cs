using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OrderAccumulator.Worker;
using OrderAccumulator.Infrastructure;

namespace OrderAccumulator.Tests;

public class AccumulatorWorkerTests
{
    [Fact]
    public async Task ExecuteAsync_StartsAcceptor()
    {
        var acceptorMock = new Mock<ISocketAcceptorWrapper>();
        var worker = new AccumulatorWorker(
            acceptorMock.Object,
            NullLogger<AccumulatorWorker>.Instance
        );

        await worker.StartAsync(CancellationToken.None);

        acceptorMock.Verify(x => x.Start(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_StopsAcceptorOnCancellation()
    {
        var acceptorMock = new Mock<ISocketAcceptorWrapper>();
        var worker = new AccumulatorWorker(
            acceptorMock.Object,
            NullLogger<AccumulatorWorker>.Instance
        );

        var cts = new CancellationTokenSource();
        await worker.StartAsync(cts.Token);
        await worker.StopAsync(CancellationToken.None);

        acceptorMock.Verify(x => x.Stop(), Times.Once);
    }
}