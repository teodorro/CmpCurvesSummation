using System;
using System.Collections.Generic;
using System.Linq; 

namespace CmpCurvesSummation.Core
{
    public interface ISummedScan
    {
        List<double[]> Data { get; }
        double StepVelocity { get; set; }
        double StepTime { get; set; }
        double StepHeight { get; }
        int VelocityLengthDimensionless { get; }
        double VelocityLength { get; }
        int AscanLengthDimensionless { get; }
        double AscanLength { get; }

    }



    public class SummedScan : ISummedScan
    {
        public List<double[]> Data { get; } = new List<double[]>();
        public double StepVelocity { get; set; } = 0.001;
        public double StepHeight { get; set; } = 0.1;
        public double StepTime { get; set; } = 1;
        
        public int VelocityLengthDimensionless => Data.Count;
        public double VelocityLength => Data.Count * StepVelocity;
        public int AscanLengthDimensionless => Data.Any() ? Data.Select(x => x.Length).Min() : -1;
        public double AscanLength => AscanLengthDimensionless * StepTime;

        public SummedScan(ICmpScan cmpScan)
        {
            Sum(cmpScan);
        }

        private void Sum(ICmpScan cmpScan)
        {
            var maxTime = cmpScan.AscanLength;
            var minVelocity = CmpMath.Instance.WaterVelocity;
            var maxVelocity = CmpMath.SpeedOfLight;
            var maxHeight = maxTime * minVelocity;
            var minHeight = 0;
            var dLength = cmpScan.AscanLengthDimensionless;
            var vLength = 200;
            var hLength = 200;

            var hStep = maxHeight / hLength;
            var vStep = (maxVelocity - minVelocity) / vLength;
            var tStep = cmpScan.StepTime;

            for (int j = 0; j < hLength; j++)
            for (int k = 0; k < vLength; k++)
            {
                var h = hStep * j;
                var v = vStep * k + minVelocity;
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
            }
            

            // make scales
            // calc row of t
            // sum and put in data. What will happen with those where there's no appropriate t?
        }
    }
}