using OrderAccumulator.Application;
using OrderAccumulator.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ExposureService>();
builder.Services.AddSingleton<FixAcceptorApplication>();
builder.Services.AddSingleton<FixSessionSettingsFactory>();
builder.Services.AddSingleton<ISocketAcceptorWrapper>(sp =>
{
    var fixApp = sp.GetRequiredService<FixAcceptorApplication>();
    var settings = sp.GetRequiredService<FixSessionSettingsFactory>().Create();
    return new SocketAcceptorWrapper(fixApp, settings);
});
builder.Services.AddHostedService<AccumulatorWorker>();

var host = builder.Build();
host.Run();