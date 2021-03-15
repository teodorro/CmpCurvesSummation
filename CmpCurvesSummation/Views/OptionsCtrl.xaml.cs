using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CmpCurvesSummation.Core;
using CmpCurvesSummation.ViewModels;

namespace CmpCurvesSummation.Views
{
    /// <summary>
    /// Interaction logic for OptionsCtrl.xaml
    /// </summary>
    public partial class OptionsCtrl : UserControl
    {
        public OptionsCtrl()
        {
            InitializeComponent();
            EventAggregator.Instance.FileLoaded += (o, args) => { this.IsEnabled = true; };
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
