using System;
using System.Collections.Generic;
using System.Linq; 

namespace CmpCurvesSummation.Core
{
    public interface ISummedScanVH
    {
        List<double[]> Data { get; }
        double StepTime { get; set; }
        double MinVelocity { get; }
        double MaxVelocity { get; }
        int AscanLengthDimensionless { get; }
        double MinHeight { get; }
        double MaxHeight { get; }
        double AscanLength { get; }
        double[,] GetDataArray();

    }



    public class SummedScanVH : ISummedScanVH
    {
        public List<double[]> Data { get; } = new List<double[]>();
        public double StepVelocity { get; set; } = 0.001;
        public double StepHeight { get; set; } = 0.1;
        public double StepTime { get; set; } = 1;
        public double MinVelocity { get; } = CmpMath.Instance.Velocity(CmpMath.WaterPermittivity);
        public double MaxVelocity { get; } = CmpMath.SpeedOfLight / 2;

        public int VelocityLengthDimensionless => Data.Count;
        public double VelocityLength => Data.Count * StepVelocity;
        public int AscanLengthDimensionless => Data.Any() ? Data.Select(x => x.Length).Min() : -1;
        public double MinHeight { get; } = 0;
        public double MaxHeight { get; private set; }
        public double AscanLength => AscanLengthDimensionless * StepTime;
        int vLength = 100;
        int hLength = 500;


        public SummedScanVH(ICmpScan cmpScan)
        {
            MaxHeight = MaxVelocity * AscanLengthDimensionless * StepTime;
            Sum(cmpScan);
        }


        private void Sum(ICmpScan cmpScan)
        {
            var maxTime = cmpScan.AscanLength;
            MaxHeight = maxTime * MaxVelocity; // ----------------------------- TODO: What depth should be chosen? This one is too high
            var dLength = cmpScan.LengthDimensionless;

            var hStep = (MaxHeight - MinHeight) / cmpScan.AscanLengthDimensionless;
            var vStep = (MaxVelocity - MinVelocity) / vLength;
            var tStep = cmpScan.StepTime;


            for (int k = 0; k < vLength; k++)
            {
                var v = vStep * k + MinVelocity;
                Data.Add(new double[hLength]);

                for (int j = 0; j < hLength; j++)
                {
                    var h = hStep * j;
                    var hodograph = new Double[dLength];
                    for (int i = 0; i < dLength; i++)
                    {
                        var d = i * cmpScan.StepDistance;
                        hodograph[i] = CmpMath.Instance.HodographLineLoza(d, h, v);
                    }

                    var sum = 0.0;
                    for (int i = 0; i < dLength; i++)
                    {
                        var tIndex = Convert.ToInt32(Math.Round(hodograph[i] / tStep));
                        if (tIndex >= 0 && tIndex < cmpScan.AscanLengthDimensionless)
                        {
                            sum += cmpScan.Data[i][tIndex];
                        }
                    }

                    Data[k][j] = sum;
                }
            }

        }

        public double[,] GetDataArray()
        {
            var res = new double[VelocityLengthDimensionless, AscanLengthDimensionless];

            for (int i = 0; i < VelocityLengthDimensionless; i++)
            for (int j = 0; j < AscanLengthDimensionless; j++)
                res[i, j] = Data[i][j];

            return res;
        }
    }
}