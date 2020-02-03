using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProcessingModule.Processing;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for StraightenSynchronizationLine2Ctrl.xaml
    /// </summary>
    public partial class StraightenSynchronizationLine2Ctrl : UserControl
    {
        private StraightenSynchronizationLine2ViewModel _viewModel;
        public double MinAmplitudeToCheck => _viewModel.MinAmplitudeToCheck;
        public double MaxAmpToBack => _viewModel.MaxAmpToBack;

        public StraightenSynchronizationLine2Ctrl(ProcessingListChangedHandler onProcessingListChanged, StraightenSynchronizationLine2 processing)
        {
            InitializeComponent();

            _viewModel = new StraightenSynchronizationLine2ViewModel(processing);
            _viewModel.ProcessingListChanged += onProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
