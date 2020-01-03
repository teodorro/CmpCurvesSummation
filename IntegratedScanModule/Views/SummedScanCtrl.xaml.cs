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
using SummedScanModule.ViewModels;

namespace SummedScanModule.Views
{
    /// <summary>
    /// Interaction logic for SummedScanCtrl.xaml
    /// </summary>
    public partial class SummedScanCtrl : UserControl
    {

        private SummedScanViewModel _viewModel;
        public SummedScanViewModel ViewModel => _viewModel;

        public SummedScanCtrl()
        {
            InitializeComponent();

            _viewModel = new SummedScanViewModel();
            DataContext = _viewModel;
        }
    }
}