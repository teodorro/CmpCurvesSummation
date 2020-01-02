using System.Windows;
using AppWithSimpleTestScan.ViewModels;
using CmpCurvesSummation.Core;

namespace AppWithSimpleTestScan.Views
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

            SetUpEvents();
        }

        private void SetUpEvents()
        {
            ToolbarControl.ViewModel.OnFileOpened += CmpScanControl.ViewModel.DataLoaded;

        }
    }
}
