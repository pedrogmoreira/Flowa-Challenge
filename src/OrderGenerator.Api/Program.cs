using OrderGenerator.Api.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<FixInitiatorService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<FixInitiatorService>());

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();