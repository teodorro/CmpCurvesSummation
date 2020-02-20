using System.Collections.Generic;
using System.Linq;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.CmpScan
{
    public class StraightenSynchronizationLine2 : ICmpScanProcessing
    {
        public string Name { get; } = "Выпрямление линии синхронизации 2";

        public override string ToString() => Name;

        public double MinNegativeAmplitudeToBegin { get; set; } = -2;
        public double MinPositiveAmplitudeToStop { get; set; } = 2;

        public int OrderIndex { get; }


        public StraightenSynchronizationLine2(int orderIndex)
        {
            OrderIndex = orderIndex;
        }


        public void Process(ICmpScan cmpScan)
        {
            var negativeIndices = GetNegativeIndicesCmpScan(cmpScan.Data);
            var positiveIndices = GetPositiveIndicesCmpScan(cmpScan.Data, negativeIndices);
            MakeOffsetsSame(cmpScan.Data, positiveIndices);
        }

        private void MakeOffsetsSame(List<double[]> cmpScanData, int[] positiveIndices)
        {
            var max = positiveIndices.Max();
            for (int i = 0; i < cmpScanData.Count; i++)
            for (int j = cmpScanData[i].Length - 1; j >= 0; j--)
                if (j - max + positiveIndices[i] >= 0)
                    cmpScanData[i][j] = cmpScanData[i][j - max + positiveIndices[i]];
                else
                    cmpScanData[i][j] = 0;
        }

        private int[] GetPositiveIndicesCmpScan(List<double[]> cmpScanData, int[] negativeIndices)
        {
            var indices = new int[cmpScanData.Count];
            for (int i = 0; i < cmpScanData.Count; i++)
                indices[i] = GetPositiveIndexAscan(cmpScanData[i], negativeIndices[i]);

            return indices;
        }

        private int GetPositiveIndexAscan(double[] ascan, int negativeIndex)
        {
            for (int i = negativeIndex; i >= 0; i--)
                if (ascan[i] >= MinPositiveAmplitudeToStop)
                    return i;
            return negativeIndex - 1;
        }

        private int[] GetNegativeIndicesCmpScan(List<double[]> cmpScanData)
        {
            var indices = new int[cmpScanData.Count];
            for (int i = 0; i < cmpScanData.Count; i++)
                indices[i] = GetNegativeIndexAscan(cmpScanData[i]);

            return indices;
        }

        private int GetNegativeIndexAscan(double[] ascan)
        {
            for (int i = 0; i < ascan.Length; i++)
                if (ascan[i] <= MinNegativeAmplitudeToBegin)
                    return i;
            return 0;
        }
    }
}