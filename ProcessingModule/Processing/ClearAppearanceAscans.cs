using System.Collections.Generic;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing
{
    public class ClearAppearanceAscans : IRawDataProcessing
    {
        public string Name { get; } = "Очистить измерения для галочки";
        public override string ToString() => Name;

        public int NumberOfAscans { get; set; } = 5;

        public void Process(ICmpScan cmpScan)
        {
            var data = cmpScan.Data;
            for (int i = 0; i < NumberOfAscans; i++)
            for (int j = 0; j < data[i].Length; j++)
                data[i][j] = 0;
        }


        /// <summary>
        /// Better to make it working
        /// </summary>
        public int DetectNumberOfAppearanceAscans(List<double> data)
        {
            double DifferencePercent = 0.001;
            return -1;
        }
    }
}