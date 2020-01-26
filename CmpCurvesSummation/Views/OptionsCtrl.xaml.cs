using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        public IEnumerable<KeyValuePair<String, Color>> NamedColors
        {
            get;
            private set;
        }

        public OptionsCtrl()
        {
            InitializeComponent();

            this.NamedColors = this.GetColors();

            _viewModel = new OptionsViewModel();
            DataContext = _viewModel;
        }

        private void ButtonSummation_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LaunchSummation();
        }

        private IEnumerable<KeyValuePair<String, Color>> GetColors()
        {
            return typeof(Colors)
                .GetProperties()
                .Where(prop =>
                    typeof(Color).IsAssignableFrom(prop.PropertyType))
                .Select(prop =>
                    new KeyValuePair<String, Color>(prop.Name, (Color)prop.GetValue(null)));
        }
    }

    public static class ColorHelper
    {
        public static IEnumerable<KeyValuePair<String, Color>> GetColors()
        {
            return typeof(Colors)
                .GetProperties()
                .Where(prop =>
                    typeof(Color).IsAssignableFrom(prop.PropertyType))
                .Select(prop =>
                    new KeyValuePair<String, Color>(prop.Name, (Color)prop.GetValue(null)));
        }
    }
}
