
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Core;

namespace GermanTraining.ViewModels;

public partial class CardsViewModel: ObservableObject
{
    /*-----------------Property-------------------*/
    [ObservableProperty]
    ExcelRow _CurrentWord = new();





    /*-----------------Command-------------------*/
    [RelayCommand]
    void GiveMeAtopic() 
    {
       
    }
}
