using System.Windows.Controls;
using ProcessingModule.Processing.CmpScan;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views.CmpScan
{
    /// <summary>
    /// Interaction logic for AddOffsetAscansCtrl.xaml
    /// </summary>
    public partial class AddOffsetAscansCtrl : UserControl
    {
        private AddOffsetAscansViewModel _viewModel;
        public int NumberOfOffsetAscans => _viewModel.NumberOfOffsetAscans;

        public AddOffsetAscansCtrl(CmpProcessingListChangedHandler onCmpProcessingListChanged, AddOffsetAscans processing)
        {
            InitializeComponent();

            _viewModel = new AddOffsetAscansViewModel(processing);
            _viewModel.ProcessingListChanged += onCmpProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
