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
        Tuple<double, double> CorrectPoint(double v, double t);
        int CheckRadius { get; }
    }



    public class SummedScanVT : ISummedScanVT
    {
        private const int _vLengthDimensionless = 100;

        public List<double[]> Data { get; } = new List<double[]>();
        public double StepVelocity => (MaxVelocity - MinVelocity) / _vLengthDimensionless;
        public double StepTime { get; }
        public double StepDistance { get; }
        public double MinVelocity { get; } = CmpMath.Instance.Velocity(CmpMath.WaterPermittivity);
        public double MaxVelocity { get; } = CmpMath.SpeedOfLight;
        public double MinTime { get; } = 0;
        public double MaxTime => MinTime + AscanLength;
        public int AscanLengthDimensionless { get; }
        public double AscanLength => AscanLengthDimensionless * StepTime;
        public int CheckRadius { get; } = 10;

        private Dictionary<double, Tuple<int, int>> _checkOrderDict = new Dictionary<double, Tuple<int, int>>();



        public SummedScanVT(ICmpScan cmpScan)
        {
            StepTime = cmpScan.StepTime;
            StepDistance = cmpScan.StepDistance;
            AscanLengthDimensionless = cmpScan.AscanLengthDimensionless;
            MinTime = cmpScan.MinTime;
            FillCheckOrderDict();
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

        //TODO:!!
        public Tuple<double, double> CorrectPoint(double v, double t)
        {
            var indV = Math.Round((v - MinVelocity) / StepVelocity);
            var indT = Math.Round(t / StepTime);

            return null;
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

        private double[,] CheckOrderMatrix()
        {
            var matrix = new double[CheckRadius * 2 + 1, CheckRadius * 2 + 1];
            var middle = CheckRadius;
            for (int i = 0; i < CheckRadius + 1; i++)
            for (int j = 0; j < CheckRadius + 1; j++)
            {
                var x = middle - i;
                var y = middle - j;
                matrix[x, y] = (x == 0 && y == 0) ? 2 : 1 / Math.Sqrt(x * x + y * y);
                x = middle + i;
                y = middle - j;
                matrix[x, y] = (x == 0 && y == 0) ? 2 : 1 / Math.Sqrt(x * x + y * y);
                x = middle - i;
                y = middle + j;
                matrix[x, y] = (x == 0 && y == 0) ? 2 : 1 / Math.Sqrt(x * x + y * y);
                x = middle + i;
                y = middle + j;
                matrix[x, y] = (x == 0 && y == 0) ? 2 : 1 / Math.Sqrt(x * x + y * y);
            }

            return matrix;
        }

        private void FillCheckOrderDict()
        {
            var middle = CheckRadius;
            var littleVal = 0.00001;
            var key = 0.0;
            for (int i = 0; i < CheckRadius + 1; i++)
            for (int j = 0; j < CheckRadius + 1; j++)
            {
                var x = middle - i;
                var y = middle - j;
                key = (x == 0 && y == 0) ? 2 : 1 / Math.Sqrt(x * x + y * y);
                for(;;)
                    if (_checkOrderDict.ContainsKey(key))
                        key -= littleVal;
                    else
                        break;
                _checkOrderDict.Add(key, new Tuple<int, int>(x, y));
                x = middle + i;
                y = middle - j;
                key = (x == 0 && y == 0) ? 2 : 1 / Math.Sqrt(x * x + y * y);
                for (; ; )
                    if (_checkOrderDict.ContainsKey(key))
                        key -= littleVal;
                    else
                        break;
                    _checkOrderDict.Add(key, new Tuple<int, int>(x, y));
                x = middle - i;
                y = middle + j;
                key = (x == 0 && y == 0) ? 2 : 1 / Math.Sqrt(x * x + y * y);
                for (; ; )
                    if (_checkOrderDict.ContainsKey(key))
                        key -= littleVal;
                    else
                        break;
                    _checkOrderDict.Add(key, new Tuple<int, int>(x, y));
                    x = middle + i;
                y = middle + j;
                key = (x == 0 && y == 0) ? 2 : 1 / Math.Sqrt(x * x + y * y);
                for (; ; )
                    if (_checkOrderDict.ContainsKey(key))
                        key -= littleVal;
                    else
                        break;
                    _checkOrderDict.Add(key, new Tuple<int, int>(x, y));
            }

            _checkOrderDict = _checkOrderDict.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
        }

        private List<Tuple<int, int>> FindExtremums(int x, int y)
        {
            var extremums = new List<Tuple<int, int>>();

            var middle = CheckRadius;
            for (int i = 0; i < CheckRadius + 1; i++)
            for (int j = 0; j < CheckRadius + 1; j++)
            {
                var x1 = x + middle - i;
                var y1 = y + middle - j;
                if (CheckIfExtremum(x1, y1))
                    extremums.Add(new Tuple<int, int>(x1, y1));
            }

            return extremums;
        }

        private bool CheckIfExtremum(int x, int y)
        {
            for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
            {
                if (x + i < 0 || x + i >= Data.Count || j + y < 0 || j + y >= AscanLength)
                    return false;
                if (i == 0 && j == 0)
                    continue;
                if (Data[x + i][y + j] > Data[x][y])
                    return false;
            }

            return true;
        }
    }
}