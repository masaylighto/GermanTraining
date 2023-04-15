

using GermanTraining.Pages;
using GermanTraining.ViewModels;
using LinqToExcel;
using Logic.Core;
using Logic.Core.DataType;
using Logic.Core.Helpers;
using Logic.Repositories;
using Logic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

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
            ConfigBuilder.AddJsonFile(Constant.PathToConfigurationFile);
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

        services.AddConfigurationEditor();
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
        string filePath = configuration.GetValue<string>("ExcelFilePath");
        if (File.Exists(configuration.GetValue<string>("ExcelFilePath")))
        {
            return new(filePath);

        }
        
        OpenFileDialog openFileDialog = new OpenFileDialog();

        openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
        if (!openFileDialog.ShowDialog().Value)
        {
            MessageBox.Show("Sorry but without an excel file you can't use this app");
            Application.Current.Shutdown();
        }
        IConfigurationEditor configurationEditor = new ConfigurationEditor();
        configurationEditor.Parse(Constant.PathToConfigurationFile);
        configurationEditor.SetValue("ExcelFilePath", openFileDialog.FileName);
        configuration["ExcelFilePath"] = openFileDialog.FileName;
        return new ExcelQueryFactory(openFileDialog.FileName);
    }

}
