using System.Windows.Controls;
using CmpScanModule.ViewModels;

namespace CmpScanModule.Views
{
    /// <summary>
    /// Interaction logic for CmpScanCtrl.xaml
    /// </summary>
    public partial class CmpScanCtrl : UserControl
    {
        private CmpScanViewModel _viewModel;
        public CmpScanViewModel ViewModel => _viewModel;

        public CmpScanCtrl()
        {
            InitializeComponent();

            _viewModel = new CmpScanViewModel();
            DataContext = _viewModel;
        }
    }
}
