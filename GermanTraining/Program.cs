

using GermanTraining.Pages;
using GermanTraining.ViewModels;
using LinqToExcel;
using Logic.Core;
using Logic.Core.Helpers;
using Logic.Repositories;
using Logic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace GermanTraining;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        IHost host = CreateHostBuilder(args);
        RunMainWindow(host);
      
    }
    
    static void RunMainWindow(IHost host) 
    {       
        host.Services.GetService<MainWindow>()?.ShowDialog();
    }
    static IHost CreateHostBuilder(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args);
        host.ConfigureAppConfiguration((ConfigBuilder) => {
            ConfigBuilder.AddEnvironmentVariables();
            ConfigBuilder.AddJsonFile("AppSettings.json");
        });
        host.ConfigureServices(AddService);        
        return host.Build();
    }
    static void AddService(HostBuilderContext context,IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();

        services.AddScoped<ArticlesPage>();
        services.AddScoped<ArticlesViewModel>();

        services.AddScoped<CardsPage>();
        services.AddScoped<CardsViewModel>();

        services.AddScoped<PhrasesPage>();        
        services.AddScoped<PhrasesViewModel>();

        services.AddSerilogLogger();

        services.AddGPTClient(CreateGPTApiConfig(context.Configuration));
        services.AddGPTService();

        services.AddExcelRepository(CreateExcelQueryFactory(context.Configuration));
        services.AddExcelService();
    }
    static GPTApiConfig CreateGPTApiConfig(IConfiguration configuration) 
    {
        GPTApiConfig config = new GPTApiConfig(); 
        configuration.GetSection("GptApiConfig").Bind(config); //fill from json file
        configuration.Bind(config);//fill from environment variable
        return config;
    }
    static ExcelQueryFactory CreateExcelQueryFactory(IConfiguration configuration)
    {
        return new(configuration.GetValue<string>("ExcelFilePath"));
    }

}
