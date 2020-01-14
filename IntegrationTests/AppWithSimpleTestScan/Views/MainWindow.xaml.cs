using System.Windows;
using AppWithSimpleTestScan.ViewModels;

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
            SetUpAutoSummation(false);
        }

        private void SetUpAutoSummation(bool auto)
        {
            OptionsControl.ViewModel.AutoSummation = auto;
        }

        private void SetUpEvents()
        {
            ToolbarControl.ViewModel.FileOpened += ProcessingControl.ViewModel.OnFileLoaded;
            ToolbarControl.ViewModel.FileOpened += OptionsControl.ViewModel.OnFileLoaded;
            ProcessingControl.ViewModel.RawCmpDataProcessed += CmpScanControl.ViewModel.OnRawCmpDataProcessed;
            ProcessingControl.ViewModel.RawCmpDataProcessed += SummedOverCurveScanControl.ViewModel.OnRawCmpDataProcessed;
            ProcessingControl.ViewModel.RawCmpDataProcessed += OptionsControl.ViewModel.OnRawCmpDataProcessed;
            SummedOverCurveScanControl.ViewModel.HodographDrawClick += CmpScanControl.ViewModel.OnHodographDrawClick;
            SummedOverCurveScanControl.ViewModel.HodographDrawClick += LayersInfoControl.ViewModel.OnHodographDrawClick;
            LayersInfoControl.ViewModel.DeleteClick += CmpScanControl.ViewModel.OnDeleteClick;
            LayersInfoControl.ViewModel.DeleteClick += SummedOverCurveScanControl.ViewModel.OnDeleteClick;
            OptionsControl.ViewModel.SummationClick += SummedOverCurveScanControl.ViewModel.OnSummationClick;
            OptionsControl.ViewModel.AutoSumCheckEvent += SummedOverCurveScanControl.ViewModel.OnAutoSummationChange;
            OptionsControl.ViewModel.PaletteChanged += CmpScanControl.ViewModel.OnPaletteChanged;
            OptionsControl.ViewModel.PaletteChanged += SummedOverCurveScanControl.ViewModel.OnPaletteChanged;
        }
    }
}
