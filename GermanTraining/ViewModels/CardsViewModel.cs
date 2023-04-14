
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Core;
using Logic.Services;
using System.Threading.Tasks;

namespace GermanTraining.ViewModels;

public partial class CardsViewModel: ObservableObject
{
    IExcelService ExcelService { get; }

    public CardsViewModel(IExcelService excelService)
    {
        ExcelService = excelService;
    }
    /*-----------------Property-------------------*/
    [ObservableProperty]
    ExcelRow _CurrentWord = new();







    /*-----------------Command-------------------*/
    [RelayCommand]
    async Task GiveMeAtopic() 
    {
        CurrentWord = await ExcelService.GetRandomWord(new());
    }
}
