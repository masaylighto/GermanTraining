

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Core;
using Logic.Services;
using System.Threading.Tasks;
using System.Windows;

namespace GermanTraining.ViewModels;

public partial class PhrasesViewModel: ObservableObject
{
    IExcelService ExcelService { get; }
    IGPTService GPTService { get; }

    public PhrasesViewModel(IExcelService excelService, IGPTService gptService)
    {
        ExcelService = excelService;
        GPTService = gptService;
        Init();
    }
    void Init()
    {
        
        NextWord();
        HideProgressBar();
        EnableButtonGrid();
    }
    /*-----------------Property-------------------*/
    [ObservableProperty]
    ExcelRow _CurrentWord = new();

    [ObservableProperty]
    string _UserPhrases;

    [ObservableProperty]
    bool _ButtonGridEnabled;

    [ObservableProperty]
    Visibility _ProgressBarVisiiblity;

    /*-----------------Command-------------------*/
    [RelayCommand]
    async Task Next()
    {
        ShowProgressBar();
        await NextWord();
        HideProgressBar();
    }
    [RelayCommand]
    async Task Submit()
    {
       
      DisableButtonGrid();
      ShowProgressBar();
    
      var result = await GPTService.ValidatePhrase(UserPhrases, GetWordWithoutTheConjugation());
      if (result.IsT0)
      {
            UserPhrases = result.AsT0;
      }
      else
      {
            UserPhrases = "Error In The Application : " + result.AsT1.Message;
      }
      EnableButtonGrid();
      HideProgressBar();
    }

    void ShowProgressBar()
    {
        ProgressBarVisiiblity = Visibility.Visible;
    }
    void HideProgressBar()
    {
        ProgressBarVisiiblity = Visibility.Hidden;
    }

    void EnableButtonGrid()
    {
        ButtonGridEnabled = true;
    }
    void DisableButtonGrid()
    {
        ButtonGridEnabled = false;
    }
    async Task NextWord()
    {
        CurrentWord = await ExcelService.GetRandomWord(new());
    }
    string GetWordWithoutTheConjugation() {

        var indexOfConjugation = CurrentWord.GermanWord.IndexOf(",");
        indexOfConjugation = indexOfConjugation == -1 ? CurrentWord.GermanWord.Length : indexOfConjugation;
        var withOutConjugationIfexist = CurrentWord.GermanWord.Substring(0, CurrentWord.GermanWord.IndexOf(","));
        return withOutConjugationIfexist;
    }
}
