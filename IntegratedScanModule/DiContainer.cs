using System;
using StructureMap;

namespace IntegratedScanModule
{
    internal class DiContainer
    {
        private static Lazy<DiContainer> _instance = new Lazy<DiContainer>(() => new DiContainer());
        public static DiContainer Instance => _instance.Value;

        public Container Container => _container;
        private Container _container = new Container(_ =>
        {
//            _.For</IFileOpener>().Use<FileOpener>();

        });
    }
}