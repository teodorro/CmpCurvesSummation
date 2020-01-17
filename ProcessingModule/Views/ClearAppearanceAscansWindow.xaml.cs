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
    /// Interaction logic for ClearAppearanceAscansWindow.xaml
    /// </summary>
    public partial class ClearAppearanceAscansWindow : Window
    {
        private ClearAppearanceAscansViewModel _viewModel;
        public int NumberOfAppearanceAscans => _viewModel.NumberOfAppearanceAscans;

        public ClearAppearanceAscansWindow(int numberOfAppearanceAscans = 5)
        {
            InitializeComponent();

            _viewModel = new ClearAppearanceAscansViewModel(numberOfAppearanceAscans);
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
