using System.Windows.Controls;
using ProcessingModule.Processing.CmpScan;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views.CmpScan
{
    /// <summary>
    /// Interaction logic for RemoveRightAscansCtrl.xaml
    /// </summary>
    public partial class RemoveRightAscansCtrl : UserControl
    {
        private RemoveRightAscansViewModel _viewModel;
//        public int NumberOfOffsetAscans => _viewModel.NumberOfAscans;

        public RemoveRightAscansCtrl(CmpProcessingListChangedHandler onCmpProcessingListChanged, RemoveRightAscans processing)
        {
            InitializeComponent();

            _viewModel = new RemoveRightAscansViewModel(processing);
            _viewModel.ProcessingListChanged += onCmpProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
