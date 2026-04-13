namespace OrderAccumulator.Worker;

public class FixSessionSettingsFactory(IConfiguration configuration)
{
    public QuickFix.SessionSettings Create()
    {
        var path = configuration["QuickFix:ConfigPath"] ?? "fix-configs/acceptor.cfg";
        return new QuickFix.SessionSettings(path);
    }
}