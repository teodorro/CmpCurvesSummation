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

        public double _maxValue = 0;
        public double MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public void InitMaxValue(ISummedScanVT summedScan)
        {
            var max = 0d;
            foreach (var ascan in summedScan.Data)
                for (int i = 0; i < ascan.Length; i++)
                    if (Math.Abs(ascan[i]) > max)
                        max = Math.Abs(ascan[i]);
            MaxValue = max;
        }

        public void Process(ISummedScanVT summedScan)
        {
            InitMaxValue(summedScan);

            foreach (var ascan in summedScan.Data)
                for (int i = 0; i < ascan.Length; i++)
                    ascan[i] = Math.Abs(ascan[i]) <= WeakValue ? 0 : ascan[i];
        }

        public int OrderIndex { get; } = 2;
    }
}