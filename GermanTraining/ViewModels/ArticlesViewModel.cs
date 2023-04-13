

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Core;
using Logic.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GermanTraining.ViewModels;

public partial class ArticlesViewModel:ObservableObject
{
    IExcelService ExcelService { get; }

    
    public ArticlesViewModel(IExcelService excelService)
    {
        ExcelService = excelService;
    }
    /*-----------------------------Property------------------------------*/
    [ObservableProperty]
    ExcelRow _CurrentNoun = new();







    /*-----------------------------Commands------------------------------*/
    [RelayCommand]
    async Task Masculine()
    {        
       await NextNoun();
    }
    [RelayCommand]
    async Task Neuter()
    {
        await NextNoun();
    }
    [RelayCommand]
    async Task Feminine()
    {
       await NextNoun();
    }

    async Task NextNoun()
    {
        CurrentNoun = await ExcelService.GetRandomNoun(new());
    }
}
