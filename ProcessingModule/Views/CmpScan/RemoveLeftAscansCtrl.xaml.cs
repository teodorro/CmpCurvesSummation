using System.Windows.Controls;
using ProcessingModule.Processing.CmpScan;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views.CmpScan
{
    /// <summary>
    /// Interaction logic for RemoveLeftAscansCtrl.xaml
    /// </summary>
    public partial class RemoveLeftAscansCtrl : UserControl
    {
        private RemoveLeftAscansViewModel _viewModel;
//        public int NumberOfOffsetAscans => _viewModel.NumberOfAscans;

        public RemoveLeftAscansCtrl(CmpProcessingListChangedHandler onCmpProcessingListChanged, RemoveLeftAscans processing)
        {
            InitializeComponent();

            _viewModel = new RemoveLeftAscansViewModel(processing);
            _viewModel.ProcessingListChanged += onCmpProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
