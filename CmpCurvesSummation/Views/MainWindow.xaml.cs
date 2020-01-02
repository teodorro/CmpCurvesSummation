using System.Windows;
using CmpCurvesSummation.Core;
using CmpCurvesSummation.ViewModels;
using GprFileService;
using StructureMap;

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
            InitializeDiContainer();

            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            //ToolbarControl.ViewModel.FileOpened += CmpScanControl.ViewModel.DataLoaded;
        }

        private void InitializeDiContainer()
        {
            DiContainer.Instance.Container = new Container(_ => { _.For<IFileOpener>().Use<FileOpener>(); });
        }
    }
}
