﻿using GermanTraining.ViewModels;
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
    /// Interaction logic for CardsPage.xaml
    /// </summary>
    public partial class CardsPage : Page
    {
        public CardsPage(CardsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        void GiveMeAtopic(object sender, RoutedEventArgs e)
        {

        }
    }
}
