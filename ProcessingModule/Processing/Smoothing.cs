using System;
using ProcessingModule;

namespace CmpCurvesSummation.Core.Processing
{
    public class Smoothing : IRawDataProcessing
    {
        public string Name { get; } = "Сглаживание";

        public void Process(ICmpScan cmpScan)
        {
            throw new NotImplementedException();
        }
    }
}