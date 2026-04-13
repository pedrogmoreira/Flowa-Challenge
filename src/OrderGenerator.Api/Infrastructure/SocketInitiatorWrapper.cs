using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;
using QuickFix.Transport;

namespace OrderGenerator.Api.Infrastructure;

public class SocketInitiatorWrapper(SessionSettings settings) : ISocketInitiatorWrapper
{
    private SocketInitiator? _initiator;

    public void Start(IApplication application)
    {
        var storeFactory = new MemoryStoreFactory();
        var logFactory = new ScreenLogFactory(settings);
        _initiator = new SocketInitiator(application, storeFactory, settings, logFactory);
        _initiator.Start();
    }

    public void Stop() => _initiator?.Stop();

    public IEnumerable<SessionID> GetSessionIDs() =>
        _initiator?.GetSessionIDs() ?? Enumerable.Empty<SessionID>();
}
