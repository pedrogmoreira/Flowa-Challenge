using QuickFix;

namespace OrderGenerator.Api.Infrastructure;

public interface ISocketInitiatorWrapper
{
    void Start(IApplication application);
    void Stop();
    IEnumerable<SessionID> GetSessionIDs();
}
