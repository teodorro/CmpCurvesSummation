using System;

namespace CmpCurvesSummation.Core.Processing
{
    public class StraightenSynchronizationLine : IRawDataProcessing
    {
        public string Name { get; } = "Выпрямление линии синхронизации";

        public void Process(ICmpScan cmpScan)
        {
            throw new System.NotImplementedException();
        }
    }
}