using System.Windows.Controls;
using ProcessingModule.Processing.CmpScan;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views.CmpScan
{
    /// <summary>
    /// Interaction logic for ClearOffsetAscansCtrl.xaml
    /// </summary>
    public partial class ClearOffsetAscansCtrl : UserControl
    {
        private ClearOffsetAscansViewModel _viewModel;
        public int NumberOfOffsetAscans => _viewModel.NumberOfOffsetAscans;


        public ClearOffsetAscansCtrl(CmpProcessingListChangedHandler onCmpProcessingListChanged, ClearOffsetAscans processing)
        {
            InitializeComponent();

            _viewModel = new ClearOffsetAscansViewModel(processing);
            _viewModel.ProcessingListChanged += onCmpProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
