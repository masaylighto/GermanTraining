

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Core;
using System.Windows;

namespace GermanTraining.ViewModels;

public partial class PhrasesViewModel: ObservableObject
{
    /*-----------------Property-------------------*/
    [ObservableProperty]
    ExcelRow _CurrentWord = new();

    [ObservableProperty]
    string _UserPhrases;




    /*-----------------Command-------------------*/
    [RelayCommand]
    void Skip()
    {

    }
    [RelayCommand]
    void Submit()
    {

    }
}
