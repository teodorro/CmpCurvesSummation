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
using ProcessingModule.ViewModels;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for StraightenSynchronizationLineWindow.xaml
    /// </summary>
    public partial class StraightenSynchronizationLineWindow : Window
    {
        private StraightenSynchronizationLineViewModel _viewModel;
        public double MinAmplitudeToCheck => _viewModel.MinAmplitudeToCheck;

        public StraightenSynchronizationLineWindow(double minAmplitudeToCheck)
        {
            InitializeComponent();

            _viewModel = new StraightenSynchronizationLineViewModel(minAmplitudeToCheck);
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
