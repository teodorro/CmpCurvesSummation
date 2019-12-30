using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace CmpCurvesSummation.Core
{
    public interface IIntegratedCmpScan
    {
        List<double[]> Data { get; }
        double StepVelocity { get; set; }
        double StepTime { get; set; }
        int VelocityLengthDimensionless { get; }
        double VelocityLength { get; }
        int AscanLengthDimensionless { get; }
        double AscanLength { get; }

    }



    public class IntegratedCmpScan : IIntegratedCmpScan
    {
        public List<double[]> Data { get; } = new List<double[]>();
        public double StepVelocity { get; set; } = 0.1;
        public double StepTime { get; set; } = 1;
        public int VelocityLengthDimensionless => Data.Count;
        public double VelocityLength => Data.Count * StepVelocity;
        public int AscanLengthDimensionless => Data.Any() ? Data.Select(x => x.Length).Min() : -1;
        public double AscanLength => AscanLengthDimensionless * StepTime;
    }
}