using System;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.SummedScan
{
    public class Absolutize : ISumScanProcessing
    {
        public string Name { get; } = "Модуль";
        public override string ToString() => Name;


        public void Process(ISummedScanVT summedScan)
        {
            foreach(var ascan in summedScan.Data)
                for (int i = 0; i < ascan.Length; i++)
                    ascan[i] = -Math.Abs(ascan[i]);
        }
    }
}