using System;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing
{
    public class Smoothing : IRawDataProcessing
    {
        public string Name { get; } = "Сглаживание";

        public override string ToString() => Name;

        public void Process(ICmpScan cmpScan)
        {
            throw new NotImplementedException();
        }
    }
}