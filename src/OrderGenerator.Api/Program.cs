using OrderGenerator.Api.Application;
using OrderGenerator.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<ISocketInitiatorWrapper>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var path = config["QuickFix:ConfigPath"] ?? "fix-configs/initiator.cfg";
    return new SocketInitiatorWrapper(new QuickFix.SessionSettings(path));
});

builder.Services.AddSingleton<FixInitiatorService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<FixInitiatorService>());
builder.Services.AddSingleton<IFixInitiatorService>(sp => sp.GetRequiredService<FixInitiatorService>());

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
