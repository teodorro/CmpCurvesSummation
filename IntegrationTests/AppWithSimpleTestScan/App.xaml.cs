using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AppWithSimpleTestScan.ViewModels;
using CmpCurvesSummation.Core;
using GprFileService;
using ProcessingModule;
using StructureMap;

namespace AppWithSimpleTestScan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DiContainer.Instance.Container = new Container(_ =>
            {
                _.For<IFileOpener>().Use<SimpleTestScanGenerator>();
                _.For<IRawDataProcessor>().Use<RawDataProcessor>();
            });
        }
    }
}
