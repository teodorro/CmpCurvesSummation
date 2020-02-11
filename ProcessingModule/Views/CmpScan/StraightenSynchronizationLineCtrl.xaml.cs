using System.Windows.Controls;
using ProcessingModule.Processing.CmpScan;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views.CmpScan
{
    /// <summary>
    /// Interaction logic for StraightenSynchronizationLineCtrl.xaml
    /// </summary>
    public partial class StraightenSynchronizationLineCtrl : UserControl
    {
        private StraightenSynchronizationLineViewModel _viewModel;
        public double MinAmplitudeToCheck => _viewModel.MinAmplitudeToCheck;

        public StraightenSynchronizationLineCtrl(CmpProcessingListChangedHandler onCmpProcessingListChanged, StraightenSynchronizationLine processing)
        {
            InitializeComponent();

            _viewModel = new StraightenSynchronizationLineViewModel(processing);
            _viewModel.ProcessingListChanged += onCmpProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
