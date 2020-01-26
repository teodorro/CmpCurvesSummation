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
    /// Interaction logic for AddOffsetAscansCtrl.xaml
    /// </summary>
    public partial class AddOffsetAscansCtrl : UserControl
    {
        private AddOffsetAscansViewModel _viewModel;
        public int NumberOfOffsetAscans => _viewModel.NumberOfOffsetAscans;

        public AddOffsetAscansCtrl(ProcessingListChangedHandler onProcessingListChanged, AddOffsetAscans processing)
        {
            InitializeComponent();

            _viewModel = new AddOffsetAscansViewModel(processing);
            _viewModel.ProcessingListChanged += onProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
