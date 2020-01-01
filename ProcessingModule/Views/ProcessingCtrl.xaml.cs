using System.Windows.Controls;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for ProcessingCtrl.xaml
    /// </summary>
    public partial class ProcessingCtrl : UserControl
    {
        private ProcessingViewModel _viewModel;


        public ProcessingCtrl()
        {
            InitializeComponent();

            _viewModel = new ProcessingViewModel();
            DataContext = _viewModel;

//            this.UpdateLayout();
        }
    }
}
