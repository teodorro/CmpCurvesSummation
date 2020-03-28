using System;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.SummedScan
{
    public class RaiseToPower : ISumScanProcessing
    {
        public string Name { get; } = "Степень";
        public override string ToString() => Name;

        public double _power = 1;
        public double Power
        {
            get => _power;
            set => _power = value;
        }

        public int OrderIndex { get; } = 3;
        

        public void Process(ISummedScanVT summedScan)
        {
            foreach (var ascan in summedScan.Data)
                for (int i = 0; i < ascan.Length; i++)
                    ascan[i] = ascan[i] == 0 ? 0 : Math.Sign(ascan[i]) * Math.Pow(Math.Abs(ascan[i]), Power);

            summedScan.NormalizeVals();
        }
    }
}