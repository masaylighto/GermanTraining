using GermanTraining.ViewModels;
using Logic.Core;
using Logic.Services;
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

namespace GermanTraining.Pages
{
    /// <summary>
    /// Interaction logic for ArticlesPage.xaml
    /// </summary>
    public partial class ArticlesPage : Page
    {

        public ArticlesPage(ArticlesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        
    }
}
