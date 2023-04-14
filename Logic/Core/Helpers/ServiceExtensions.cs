using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Logic.Core.Helpers;

public static class ServiceExtensions
{
    public static void AddSerilogLogger(this IServiceCollection services)
    {
      var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("Log.txt")
            .CreateLogger();
        services.AddSingleton<Serilog.ILogger>(logger);
        services.AddScoped<ILogger,Logger>();
    }
}