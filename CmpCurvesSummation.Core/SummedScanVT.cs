using System;
using System.Collections.Generic;
using System.Linq;

namespace CmpCurvesSummation.Core
{
    /// <summary>
    /// Data after summation process with (velocity, time) scale
    /// </summary>
    public interface ISummedScanVT
    {
        List<double[]> Data { get; }
        double StepTime { get;  }
        double StepDistance { get; }
        double MinVelocity { get; }
        double MaxVelocity { get; }
        double MinTime { get; }
        double MaxTime { get; }
        int AscanLengthDimensionless { get; }
        double AscanLength { get; }
        double[,] GetDataArray();
        void Sum(ICmpScan cmpScan);
    }



    public class SummedScanVT : ISummedScanVT
    {
        private const int _vLengthDimensionless = 100;

        public List<double[]> Data { get; } = new List<double[]>();
        public double StepVelocity { get; set; } = 0.001;
        public double StepTime { get; }
        public double StepDistance { get; }
        public double MinVelocity { get; } = CmpMath.Instance.Velocity(CmpMath.WaterPermittivity);
        public double MaxVelocity { get; } = CmpMath.SpeedOfLight;
        public double MinTime { get; } = 0;
        public double MaxTime => MinTime + AscanLength;
        public int AscanLengthDimensionless { get; }
        public double AscanLength => AscanLengthDimensionless * StepTime;


        public SummedScanVT(ICmpScan cmpScan)
        {
            StepTime = cmpScan.StepTime;
            StepDistance = cmpScan.StepDistance;
            AscanLengthDimensionless = cmpScan.AscanLengthDimensionless;
            MinTime = cmpScan.MinTime;

//            Sum(cmpScan);
        }
        

        public void Sum(ICmpScan cmpScan)
        {
            double v;
            double h;
            var vStep = (MaxVelocity - MinVelocity) / _vLengthDimensionless;

            for (int p = 0; p < _vLengthDimensionless; p++)
            {
                CmpProgressBar.Instance.ProgressValue = p;
                v = vStep * p + MinVelocity;
                Data.Add(new double[AscanLengthDimensionless]);
                for (int j = 0; j < AscanLengthDimensionless; j++)
                {
                    var t = j * StepTime + MinTime;
                    h = v * t / 2;
                    Data[p][j] = CalcSumForVelocityAndDepth(cmpScan, h, v);
                }
            }
        }

        private double CalcSumForVelocityAndDepth(ICmpScan cmpScan, double h, double v)
        {
            double sum = 0;
            for (int i = 0; i < cmpScan.LengthDimensionless; i++)
            {
                var d = i * StepDistance;
                if (double.IsNaN(h) || h < 0)
                    continue;
                var t = CmpMath.Instance.HodographLineLoza(d, h, v);
                var tIndex = Convert.ToInt32(Math.Round(t / StepTime - cmpScan.MinTime));
                if (tIndex >= cmpScan.MinTime && tIndex < cmpScan.MaxTime)
                    sum += cmpScan.Data[i][tIndex];
            }

            return sum;
        }

        public double[,] GetDataArray()
        {
            var res = new double[_vLengthDimensionless, AscanLengthDimensionless];

            for (int i = 0; i < _vLengthDimensionless; i++)
            for (int j = 0; j < AscanLengthDimensionless; j++)
                res[i, j] = Data[i][j];

            return res;
        }
    }
}