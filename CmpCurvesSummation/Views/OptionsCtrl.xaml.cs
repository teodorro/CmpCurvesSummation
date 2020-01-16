using System.Windows;
using System.Windows.Controls;
using CmpCurvesSummation.ViewModels;

namespace CmpCurvesSummation.Views
{
    /// <summary>
    /// Interaction logic for OptionsCtrl.xaml
    /// </summary>
    public partial class OptionsCtrl : UserControl
    {
        private OptionsViewModel _viewModel;
        public OptionsViewModel ViewModel => _viewModel;

        public OptionsCtrl()
        {
            InitializeComponent();

            _viewModel = new OptionsViewModel();
            DataContext = _viewModel;
        }

        private void ButtonSummation_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LaunchSummation();
        }
    }
}
