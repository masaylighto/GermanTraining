using Logic.Core.DataType;
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
            .WriteTo.File(Constant.PathToLogFile)
            .CreateLogger();
        services.AddSingleton<Serilog.ILogger>(logger);
        services.AddScoped<ILogger,Logger>();
    }
    public static void AddConfigurationEditor(this IServiceCollection services)
    {
        services.AddScoped<IConfigurationEditor, ConfigurationEditor>();
    }
}