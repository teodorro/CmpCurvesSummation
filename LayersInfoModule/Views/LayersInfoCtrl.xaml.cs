﻿using System;
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
using LayersInfoModule.ViewModels;

namespace LayersInfoModule.Views
{
    /// <summary>
    /// Interaction logic for LayersInfoCtrl.xaml
    /// </summary>
    public partial class LayersInfoCtrl : UserControl
    {
        private LayersInfoViewModel _viewModel;
        public LayersInfoViewModel ViewModel => _viewModel;

        public LayersInfoCtrl()
        {
            InitializeComponent();

            _viewModel = new LayersInfoViewModel();
            DataContext = _viewModel;

        }
    }
}
