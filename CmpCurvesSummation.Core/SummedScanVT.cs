using System;
using System.Collections.Generic;
using System.Linq;

namespace CmpCurvesSummation.Core
{
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

    }



    public class SummedScanVT : ISummedScanVT
    {
        private const int _vLengthDimensionless = 100;

        public List<double[]> Data { get; } = new List<double[]>();
        public double StepVelocity { get; set; } = 0.001;
        public double StepTime { get; }
        public double StepDistance { get; }
        public double MinVelocity { get; } = CmpMath.Instance.Velocity(CmpMath.WaterPermittivity);
        public double MaxVelocity { get; } = CmpMath.SpeedOfLight / 2;
        public double MinTime { get; } = 0;
        public double MaxTime { get; }
        public int AscanLengthDimensionless { get; }
        public double AscanLength => AscanLengthDimensionless * StepTime;


        public SummedScanVT(ICmpScan cmpScan)
        {
            StepTime = cmpScan.StepTime;
            StepDistance = cmpScan.StepDistance;
            MaxTime = cmpScan.AscanLengthDimensionless * StepTime;
            AscanLengthDimensionless = cmpScan.AscanLengthDimensionless;

            Sum(cmpScan);
        }

        //        private void Sum(ICmpScan cmpScan)
        //        {
        //            double t0;
        //            double d0;
        //            double v;
        //            double d;
        //            double h;
        //            double t;
        //            double sum;
        //            var vStep = (MaxVelocity - MinVelocity) / _vLengthDimensionless;
        //
        //            for (int p = 0; p < _vLengthDimensionless; p++)
        //            {
        //                v = vStep * p + MinVelocity;
        //                Data.Add(new double[AscanLengthDimensionless]);
        //                for (int j = 0; j < AscanLengthDimensionless; j++)
        //                {
        //                    sum = 0;
        //
        //                    t0 = j * StepTime;
        //                    for (int i = 0; i < cmpScan.Length; i++)
        //                    {
        //                        d0 = i * StepDistance;
        //
        //                        h = Math.Sqrt(v * v * Math.Pow(t0 + d0 / CmpMath.SpeedOfLight, 2) - d0 * d0 / 4);
        //                        if (double.IsNaN(h))
        //                            continue;
        //                        for (int q = 0; q < cmpScan.Length; q++)
        //                        {
        //                            d = q * StepDistance;
        //                            t = 1 / v * Math.Sqrt(h * h + d * d / 4) - d / CmpMath.SpeedOfLight;
        //                            var tIndex = Convert.ToInt32(Math.Round(t / StepTime));
        //                            if (tIndex >= 0 && tIndex < cmpScan.AscanLengthDimensionless)
        //                            {
        //                                sum += cmpScan.Data[i][tIndex];
        //                            }
        //                        }
        //                    }
        //
        //                    Data[p][j] = sum;
        //                }
        //            }
        //        }

//        private void Sum(ICmpScan cmpScan)
//        {
//            double v;
//            double d;
//            double h;
//            double t;
//            double sum;
//            var vStep = (MaxVelocity - MinVelocity) / _vLengthDimensionless;
//
//            for (int p = 0; p < _vLengthDimensionless; p++)
//            {
//                v = vStep * p + MinVelocity;
//                Data.Add(new double[AscanLengthDimensionless]);
//                for (int j = 0; j < AscanLengthDimensionless; j++)
//                {
//                    sum = 0;
//                    t = j * StepTime;
//                    h = v * t;
//                    for (int i = 0; i < cmpScan.Length; i++)
//                    {
//                        d = i * StepDistance;
//
//                        if (double.IsNaN(h))
//                            continue;
//                        t = 1 / v * Math.Sqrt(h * h + d * d / 4) - d / CmpMath.SpeedOfLight;
//                        var tIndex = Convert.ToInt32(Math.Round(t / StepTime));
//                        if (tIndex >= 0 && tIndex < cmpScan.AscanLengthDimensionless)
//                        {
//                            sum += cmpScan.Data[i][tIndex];
//                        }
//                        
//                    }
//
//                    Data[p][j] = sum ;
//                }
//            }
//        }

        private void Sum(ICmpScan cmpScan)
        {
            double t0;
            double v;
            double d;
            double h;
            double t;
            double sum;
            var vStep = (MaxVelocity - MinVelocity) / _vLengthDimensionless;

            for (int p = 0; p < _vLengthDimensionless; p++)
            {
                v = vStep * p + MinVelocity;
                Data.Add(new double[AscanLengthDimensionless]);
                for (int j = 0; j < AscanLengthDimensionless; j++)
                {
                    sum = 0;
                    t0 = j * StepTime;
                    h = v * t0;
                    for (int i = 0; i < cmpScan.LengthDimensionless; i++)
                    {
                        d = i * StepDistance;

                        if (double.IsNaN(h))
                            continue;
                        t = 1 / v * Math.Sqrt(h * h + d * d / 4) - d / CmpMath.SpeedOfLight;
                        var tIndex = Convert.ToInt32(Math.Round(t / StepTime));
                        if (tIndex >= 0 && tIndex < cmpScan.AscanLengthDimensionless)
                        {
                            sum += cmpScan.Data[i][tIndex];
                        }

                    }

                    Data[p][j] = sum;
                }
            }
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