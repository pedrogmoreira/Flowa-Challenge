using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;
using QuickFix.Transport;
using OrderAccumulator.Application;

namespace OrderAccumulator.Worker;

public class SocketAcceptorWrapper : ISocketAcceptorWrapper
{
    private readonly ThreadedSocketAcceptor _acceptor;

    public SocketAcceptorWrapper(
        FixAcceptorApplication fixAcceptorApplication,
        SessionSettings settings)
    {
        var storeFactory = new MemoryStoreFactory();
        var logFactory = new ScreenLogFactory(settings);
        _acceptor = new ThreadedSocketAcceptor(fixAcceptorApplication, storeFactory, settings, logFactory);
    }

    public void Start() => _acceptor.Start();
    public void Stop() => _acceptor.Stop();
}