

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Core;
using Logic.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
namespace GermanTraining.ViewModels;

public partial class ArticlesViewModel:ObservableObject
{
    IExcelService ExcelService { get; }

    
    public ArticlesViewModel(IExcelService excelService)
    {
        ExcelService = excelService;
        Init();
    }
    void Init() {
        NextNoun().GetAwaiter();
        EnableArticleButtons();
    }
    /*-----------------------------Property------------------------------*/
    [ObservableProperty]
    ExcelRow _CurrentNoun = new();

    [ObservableProperty]
    string _NotificationLabelContent;

    [ObservableProperty]
    Brush _NotificationBorderColor;

    [ObservableProperty]
    Visibility _NotificationBorderVisiblity;

    [ObservableProperty]
    bool _IsArticleButtonClickable;



    /*-----------------------------Commands------------------------------*/
    [RelayCommand]
    async Task Masculine()
    {
        DisableArticleButtons();
        await ShowArticleNotification("Der");
        await NextNoun();
        EnableArticleButtons();
    }
    [RelayCommand]
    async Task Neuter()
    {
        DisableArticleButtons();
        await ShowArticleNotification("Das");
        await NextNoun();
        EnableArticleButtons();
    }
    [RelayCommand]
    async Task Feminine()
    {
        DisableArticleButtons();
        await ShowArticleNotification("Die");
        await NextNoun();
        EnableArticleButtons();
    }

    async Task ShowArticleNotification(string enteredArticle)
    {
        if (IsCorrectArticle(enteredArticle))
        {
            SetNotificationBorderColor(Brushes.LightGreen);
            ShowNotification($"Well Done");
        }
        else
        {
            SetNotificationBorderColor(Brushes.PaleVioletRed);
            ShowNotification($"The Correct Article is {CurrentNoun.Article}");
        }
        await Task.Delay(1000);
        HideNotification();

    }
    void DisableArticleButtons()
    {
        IsArticleButtonClickable = false;
    }
    void EnableArticleButtons()
    {
        IsArticleButtonClickable = true;
    }
    void SetNotificationBorderColor(Brush Color)
    {     
        NotificationBorderColor = Color;
    }

    void  ShowNotification(string message)
    {
        NotificationBorderVisiblity = Visibility.Visible;
        NotificationLabelContent= message;
    }
    void HideNotification()
    {
        NotificationBorderVisiblity = Visibility.Hidden;
        NotificationLabelContent = string.Empty;
    }
    bool IsCorrectArticle(string UsedArticle)
    {
        return CurrentNoun.Article!.Equals(UsedArticle, System.StringComparison.OrdinalIgnoreCase);
    }

    async Task NextNoun()
    {
        CurrentNoun = await ExcelService.GetRandomNoun(new());
    }
}
