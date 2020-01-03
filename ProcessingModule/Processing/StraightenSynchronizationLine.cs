using System;
using System.Collections.Generic;
using System.Linq;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing
{
    public class StraightenSynchronizationLine : IRawDataProcessing
    {
        public string Name { get; } = "Выпрямление линии синхронизации";

        public override string ToString() => Name;

        public void Process(ICmpScan cmpScan)
        {
            var firstExtremumIndices = GetFirstExtremumIndices(cmpScan.Data);
            MakeFirstExtremumIndexSame(firstExtremumIndices, cmpScan.Data);
        }

        private void MakeFirstExtremumIndexSame(int[] firstExtremumIndices, List<double[]> cmpScanData)
        {
            var maxIndex = firstExtremumIndices.Max();
            for (int i = 0; i < cmpScanData.Count; i++)
            {
                var ascan = cmpScanData[i];
                var offset = maxIndex - firstExtremumIndices[i];
                for (int j = ascan.Length - 1; j >= 0; j--)
                {
                    ascan[j] = j - offset >= 0 ? ascan[j - offset] : 0;
                }
            }
        }

        private int[] GetFirstExtremumIndices(List<double[]> cmpScanData)
        {
            var firstExtremumIndices = new int[cmpScanData.Count];
            for (int i = 0; i < cmpScanData.Count; i++)
            {
                var ascan = cmpScanData[i];
                var firstExtremumIndex = 0;

                for (int j = 1; j < ascan.Length - 1; j++)
                {
                    if (ascan[j] > ascan[j - 1] && ascan[j] > ascan[j + 1] // max
                    || ascan[j] < ascan[j - 1] && ascan[j] < ascan[j + 1]) // min
                    {
                        firstExtremumIndex = j;
                        break;
                    }
                }

                firstExtremumIndices[i] = firstExtremumIndex;
            }

            return firstExtremumIndices;
        }

    }
}