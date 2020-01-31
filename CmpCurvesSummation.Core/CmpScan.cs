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
        List<double[]> RawData { get; }
        double StepDistance { get; set; }
        double StepTime { get; set; }
        int LengthDimensionless { get; }
        double Length { get; }
        int AscanLengthDimensionless { get; }
        double AscanLength { get; }
        double MinTime { get; set; }
        double MaxTime { get; }

        void CopyRawDataToProcessed();
    }



    public class CmpScan : ICmpScan
    {
        public const double DefaultStepDistance = 0.1;
        public const double DefaultStepTime = 1;

        public List<double[]> Data { get; } = new List<double[]>();
        public List<double[]> RawData { get; } = new List<double[]>();
        public double StepDistance { get; set; } = DefaultStepDistance;
        public double StepTime { get; set; } = DefaultStepTime;
        public int LengthDimensionless => Data.Count;
        public double Length => Data.Count * StepDistance;
        public int AscanLengthDimensionless => Data.Any() ? Data.Select(x => x.Length).Min() : -1;
        public double AscanLength => AscanLengthDimensionless * StepTime;

        private double _minTimeDimensionless = 0;
        public double MinTime
        {
            get => _minTimeDimensionless * StepTime;
            set => _minTimeDimensionless = value / StepTime; 
        }
        public double MaxTime => MinTime + AscanLength;

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
