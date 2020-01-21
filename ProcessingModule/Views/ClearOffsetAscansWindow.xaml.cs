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
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for ClearOffsetAscansWindow.xaml
    /// </summary>
    public partial class ClearOffsetAscansWindow : Window
    {
        private ClearOffsetAscansViewModel _viewModel;
        public int NumberOfOffsetAscans => _viewModel.NumberOfOffsetAscans;


        public ClearOffsetAscansWindow(int numberOfOffsetAscans, ProcessingListChangedHandler onProcessingListChanged)
        {
            InitializeComponent();

            _viewModel = new ClearOffsetAscansViewModel(numberOfOffsetAscans);
            _viewModel.ProcessingListChanged += onProcessingListChanged;
            DataContext = _viewModel;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
