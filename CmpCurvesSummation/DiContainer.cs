using System;
using StructureMap;
using ToolbarModule.FileDialogs;

namespace CmpCurvesSummation
{
    internal class DiContainer
    {
        private Lazy<DiContainer> _instance = new Lazy<DiContainer>(() => new DiContainer());
        public DiContainer Instance => _instance.Value;

        public Container Container => _container;
        private Container _container = new Container(_ =>
        {
            _.For<IFileOpener>().Use<FileOpener>();

        });
    }
}