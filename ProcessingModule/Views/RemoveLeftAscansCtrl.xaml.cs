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
    /// Interaction logic for RemoveLeftAscansCtrl.xaml
    /// </summary>
    public partial class RemoveLeftAscansCtrl : UserControl
    {
        private RemoveLeftAscansViewModel _viewModel;
//        public int NumberOfOffsetAscans => _viewModel.NumberOfAscans;

        public RemoveLeftAscansCtrl(ProcessingListChangedHandler onProcessingListChanged, RemoveLeftAscans processing)
        {
            InitializeComponent();

            _viewModel = new RemoveLeftAscansViewModel(processing);
            _viewModel.ProcessingListChanged += onProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
