using System;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.SummedScan
{
    public class RaiseToPower : ISumScanProcessing
    {
        public string Name { get; } = "Степень";
        public override string ToString() => Name;

        private double _power = 1;
        public double Power
        {
            get => _power;
            set
            {
                _power = value;
            }
        }

        public void Process(ISummedScanVT summedScan)
        {
            foreach (var ascan in summedScan.Data)
                for (int i = 0; i < ascan.Length; i++)
                    ascan[i] = Math.Sign(ascan[i]) * Math.Pow(Math.Abs(ascan[i]), Power);
        }
    }
}