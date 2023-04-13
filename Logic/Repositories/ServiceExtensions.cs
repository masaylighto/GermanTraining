

using LinqToExcel;
using Logic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Repositories;

public static class ServiceExtensions
{
    public static void AddExcelRepository(this IServiceCollection services)
    {
        services.AddScoped<IExcelRepo, ExcelRepo>();
        services.AddSingleton<ExcelQueryFactory>(new ExcelQueryFactory("D:\\Learn\\German Words.xlsx"));
    }
}
