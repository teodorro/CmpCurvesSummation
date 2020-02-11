using System;
using System.Collections.Generic;
using System.Linq;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Processing.CmpScan
{
    public class StraightenSynchronizationLine : ICmpScanProcessing
    {
        public string Name { get; } = "Выпрямление линии синхронизации";

        public override string ToString() => Name;

        public double MinAmplitudeToCheck { get; set; } = 1;


        public void Process(ICmpScan cmpScan)
        {
            var firstExtremumIndices = GetFirstExtremumIndices2(cmpScan.Data);
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

        private int[] GetFirstExtremumIndices2(List<double[]> cmpScanData)
        {
            var firstExtremumIndices = new int[cmpScanData.Count];
            for (int i = 0; i < cmpScanData.Count; i++)
            {
                var ascan = cmpScanData[i];
                firstExtremumIndices[i] = GetFirstExtremumIndex(ascan);
            }

            return firstExtremumIndices;
        }


        private int GetFirstExtremumIndex(double[] ascan)
        {
            if (Math.Abs(ascan[0]) > Math.Abs(ascan[1]) && Math.Abs(ascan[0]) >= MinAmplitudeToCheck)
                return 0;
            for (int i = 1; i < ascan.Length; i++)
            {
                if (Math.Abs(ascan[i]) > Math.Abs(ascan[i - 1]) && Math.Abs(ascan[i]) >= MinAmplitudeToCheck)
                {
                    if (Math.Abs(ascan[i + 1]) > Math.Abs(ascan[i]))
                        continue;
                    if (Math.Abs(ascan[i + 1]) < Math.Abs(ascan[i]))
                        return i;
                    for (int j = i + 1; j < ascan.Length; j++)
                    {
                        if (Math.Abs(ascan[j]) < Math.Abs(ascan[i]))
                            return i + (j - i) / 2;
                        if (Math.Abs(ascan[j]) > Math.Abs(ascan[i]))
                            break;
                    }
                }
                else if (ascan[i] < ascan[i - 1] && Math.Abs(ascan[i]) >= MinAmplitudeToCheck)
                {
                    if (Math.Abs(ascan[i + 1]) < Math.Abs(ascan[i]))
                        continue;
                    if (Math.Abs(ascan[i + 1]) > Math.Abs(ascan[i]))
                        return i;
                    for (int j = i + 1; j < ascan.Length; j++)
                    {
                        if (Math.Abs(ascan[j]) > Math.Abs(ascan[i]))
                            return i + (j - i) / 2;
                        if (Math.Abs(ascan[j]) < Math.Abs(ascan[i]))
                            break;
                    }
                }
            }

            return 0;
        }


        
        private bool AreEqual(double x, double y)
        {
            return Math.Abs(x - y) < 0.0001;
        }
    }
}