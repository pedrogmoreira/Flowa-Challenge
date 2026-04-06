using OrderAccumulator.Application;
using OrderAccumulator.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ExposureService>();
builder.Services.AddSingleton<FixApplication>();
builder.Services.AddHostedService<AccumulatorWorker>();

var host = builder.Build();
host.Run();
