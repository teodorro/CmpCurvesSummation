using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CmpCurvesSummation.Core;
using CmpCurvesSummation.ViewModels;

namespace CmpCurvesSummation.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
        }

        public void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            (DataContext as MainWindowViewModel).Title = e.Filename;
        }
    }
}
