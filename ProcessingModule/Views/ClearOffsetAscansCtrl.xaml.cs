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
using CmpCurvesSummation.Core;
using ProcessingModule.Processing;
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for ClearOffsetAscansCtrl.xaml
    /// </summary>
    public partial class ClearOffsetAscansCtrl : UserControl
    {
        private ClearOffsetAscansViewModel _viewModel;
        public int NumberOfOffsetAscans => _viewModel.NumberOfOffsetAscans;


        public ClearOffsetAscansCtrl(ProcessingListChangedHandler onProcessingListChanged, ClearOffsetAscans processing)
        {
            InitializeComponent();

            _viewModel = new ClearOffsetAscansViewModel(processing);
            _viewModel.ProcessingListChanged += onProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
