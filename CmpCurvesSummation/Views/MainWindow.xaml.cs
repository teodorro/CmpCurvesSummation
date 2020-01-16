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

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            //ToolbarControl.ViewModel.FileOpened += CmpScanControl.ViewModel.DataLoaded;
        }
    }
}
