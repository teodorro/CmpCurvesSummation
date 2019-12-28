using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmpCurvesSummation.Core
{
    /// <summary>
    /// Data to work with
    /// </summary>
    /// <remarks>CMP means Common Middle Point</remarks>
    public interface ICmpScan
    {
        List<double[]> Data { get; }
        List<int[]> RawData { get; }
        double StepX { get; set; }
        double StepTime { get; set; }
        int LengthDimensionless { get; }
        double Length { get; }
        int AscanLengthDimensionless { get; }
        double AscanLength { get; }

        void CopyRawDataToProcessed();
    }


    public class CmpScan : ICmpScan
    {
        public List<double[]> Data { get; } = new List<double[]>();
        public List<int[]> RawData { get; } = new List<int[]>();
        public double StepX { get; set; } = 0.1;
        public double StepTime { get; set; } = 1;
        public int LengthDimensionless => Data.Count;
        public double Length => Data.Count * StepX;
        public int AscanLengthDimensionless => Data.Any() ? Data.Select(x => x.Length).Min() : -1;
        public double AscanLength => AscanLengthDimensionless * StepTime;

        public void CopyRawDataToProcessed()
        {
            Data.Clear();
            foreach (var rawAscan in RawData)
            {
                var ascan = new double[rawAscan.Length];
                Data.Add(ascan);
                for (int i = 0; i < rawAscan.Length; i++)
                    ascan[i] = rawAscan[i];
            }
        }
    }
}
