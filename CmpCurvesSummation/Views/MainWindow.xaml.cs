using System.Windows;
using CmpCurvesSummation.ViewModels;

namespace CmpCurvesSummation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMainWindowViewModel _viewModel;
        public IMainWindowViewModel ViewModel => _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            ToolbarControl.ViewModel.OnFileOpened += CmpScanControl.ViewModel.DataLoaded;
        }
    }
}
