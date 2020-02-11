using System.Windows.Controls;
using ProcessingModule.Processing.CmpScan;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views.CmpScan
{
    /// <summary>
    /// Interaction logic for StraightenSynchronizationLine2Ctrl.xaml
    /// </summary>
    public partial class StraightenSynchronizationLine2Ctrl : UserControl
    {
        private StraightenSynchronizationLine2ViewModel _viewModel;
        public double MinAmplitudeToCheck => _viewModel.MinAmplitudeToCheck;
        public double MaxAmpToBack => _viewModel.MaxAmpToBack;

        public StraightenSynchronizationLine2Ctrl(CmpProcessingListChangedHandler onCmpProcessingListChanged, StraightenSynchronizationLine2 processing)
        {
            InitializeComponent();

            _viewModel = new StraightenSynchronizationLine2ViewModel(processing);
            _viewModel.ProcessingListChanged += onCmpProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
