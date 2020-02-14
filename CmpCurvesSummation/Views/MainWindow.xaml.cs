using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CmpCurvesSummation.Core;
using CmpCurvesSummation.ViewModels;

namespace CmpCurvesSummation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            EventAggregator.Instance.FileLoaded += OnFileLoaded;

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

        }

        public void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            _viewModel.Title = e.Filename;
        }
    }
}
