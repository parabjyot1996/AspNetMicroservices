using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureAppConfiguration((hostingContext, config) => {
    config.AddJsonFile(
        $"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json",
        true, 
        true);
});
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddOcelot();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseOcelot();

app.Run();
