using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using ToolbarModule.FileDialogs;

namespace ToolbarModule
{
    internal class DiContainer
    {
        private static Lazy<DiContainer> _instance = new Lazy<DiContainer>(() => new DiContainer());
        public static DiContainer Instance => _instance.Value;

        public Container Container => _container;
        private Container _container = new Container(_ =>
        {
            _.For<IFileOpener>().Use<FileOpener>();

        });
    }
}
