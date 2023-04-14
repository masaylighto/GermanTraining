
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Services;

public static class ServiceExtensions
{
    public static void AddGPTService(this IServiceCollection services)
    {
        services.AddScoped<IGPTService, GPTService>();
    }
    public static void AddExcelService(this IServiceCollection services)
    {
        services.AddScoped<IExcelService, ExcelService>();
       
    }
}
