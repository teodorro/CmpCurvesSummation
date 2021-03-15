using System;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.SummedScan
{
    public class HideWeakValues : ISumScanProcessing
    {
        public string Name { get; } = "Скрыть слабые амплитуды";
        public override string ToString() => Name;

        public double _weakValue = 0;
        public double WeakValue
        {
            get => _weakValue;
            set => _weakValue = value;
        }

        public void Process(ISummedScanVT summedScan)
        {
            foreach (var ascan in summedScan.Data)
                for (int i = 0; i < ascan.Length; i++)
                    ascan[i] = Math.Abs(ascan[i]) <= WeakValue 
                        ? 0 
                        : (ascan[i] > 0 
                            ? ascan[i] - WeakValue
                            : ascan[i] + WeakValue);

            summedScan.NormalizeVals();
        }

        public int OrderIndex { get; } = 2;
    }
}