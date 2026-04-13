namespace OrderAccumulator.Infrastructure;

public interface ISocketAcceptorWrapper
{
    void Start();
    void Stop();
}