﻿

using GermanTraining.Pages;
using GermanTraining.ViewModels;
using Logic.Repositories;
using Logic.Services;
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

        services.AddExcelRepository();
        services.AddExcelService();
    }

}
