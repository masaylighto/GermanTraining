

using LinqToExcel;
using Logic.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Repositories;

public static class ServiceExtensions
{
    public static void AddGPTClient(this IServiceCollection services,GPTApiConfig config)
    {
        services.AddSingleton(config);
        services.AddScoped<IGPTClient, GPTClient>();
        
    }
    public static void AddExcelRepository(this IServiceCollection services, ExcelQueryFactory ExcelQueryFactory)
    {
        services.AddScoped<IExcelRepo, ExcelRepo>();
        services.AddSingleton(ExcelQueryFactory);
    }
}
