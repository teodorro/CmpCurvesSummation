using System;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.CmpScan
{
    public class LogarithmProcessing : ICmpScanProcessing
    {
        public string Name { get; } = "Логарифм амплитуды";

        public override string ToString() => Name;
        

        public void Process(ICmpScan cmpScan)
        {
            var data = cmpScan.Data;
            for (int i = 0; i < data.Count; i++)
            for (int j = 0; j < data[i].Length; j++)
                data[i][j] = Math.Log(Math.Abs(data[i][j]) + 1) * Math.Sign(data[i][j]);
        }
    }
}