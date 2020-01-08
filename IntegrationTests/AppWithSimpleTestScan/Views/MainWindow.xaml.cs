using System.Windows;
using AppWithSimpleTestScan.ViewModels;
using SummedScanModule.Views;

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
            ToolbarControl.ViewModel.FileOpened += ProcessingControl.ViewModel.OnFileLoaded;
            ProcessingControl.ViewModel.RawCmpDataProcessed += CmpScanControl.ViewModel.OnRawCmpDataProcessed;
            ToolbarControl.ViewModel.FileOpened += StepsControl.ViewModel.OnFileLoaded;
            ProcessingControl.ViewModel.RawCmpDataProcessed += SummedOverCurveScanControl.ViewModel.OnRawCmpDataProcessed;
            SummedOverCurveScanControl.ViewModel.HodographDrawClick += CmpScanControl.ViewModel.OnHodographDrawClick;
            SummedOverCurveScanControl.ViewModel.HodographDrawClick += LayersInfoControl.ViewModel.OnHodographDrawClick;
        }
    }
}
