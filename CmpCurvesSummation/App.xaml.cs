using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CmpCurvesSummation.Core;
using GprFileService;
using ProcessingModule;
using StructureMap;

namespace CmpCurvesSummation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeDiContainer();
        }

        private static void InitializeDiContainer()
        {
            DiContainer.Instance.Container = new Container(_ =>
            {
                _.For<IFileOpener>().Use<FileOpener>();
                _.For<ICmpScanProcessor>().Use<CmpScanProcessor>();
            });
        }
    }
}
