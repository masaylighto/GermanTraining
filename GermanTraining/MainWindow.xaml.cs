using GermanTraining.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GermanTraining
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ArticlesPage ArticlesPage { get; }
        public CardsPage CardsPage { get; }
        public PhrasesPage PhrasesPage { get; }

        public MainWindow(ArticlesPage articlesPage ,CardsPage cardsPage, PhrasesPage phrasesPage)
        {
            InitializeComponent();
            ArticlesPage = articlesPage;
            CardsPage = cardsPage;
            PhrasesPage = phrasesPage;
        }

        void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void LoadCardPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Content= CardsPage;
        }

        void LoadPhrasesPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = PhrasesPage;
        }

        void LoadArticlesPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Content= ArticlesPage;
        }
    }
}
