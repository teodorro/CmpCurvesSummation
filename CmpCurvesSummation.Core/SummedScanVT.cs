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
        int CheckRadius { get; }
        List<Tuple<double, double>> Layers { get; }
        int MinimalTimeBetweenLayers { get; }

        double[,] GetDataArray();
        void Sum(ICmpScan cmpScan);
        Tuple<double, double> CorrectPoint(double velocity, double time);
        void AddLayer(double velocity, double time);
        void RemoveLayersAround(double velocity, double time);

        event RefreshLayersHandler RefreshLayers;
    }



    public class SummedScanVT : ISummedScanVT
    {
        private const int _vLengthDimensionless = 100;

        private Dictionary<double, Tuple<int, int>> _checkOrderDict = new Dictionary<double, Tuple<int, int>>();

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
        public List<Tuple<double, double>> Layers { get; } = new List<Tuple<double, double>>();
        public int MinimalTimeBetweenLayers { get; } = 10;

        public event RefreshLayersHandler RefreshLayers;


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
                    var t = Time(j);
                    h = v * t / 2;
                    var sum = CalcSumForVelocityAndDepth(cmpScan, h, v);
//                    Data[p][j] = Math.Sign(sum) * Math.Log(Math.Abs(sum) + 1); // log for sum works not good
                    Data[p][j] = sum;
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
                var tIndex = IndexTime(t);
                if (tIndex >= 0 && tIndex < cmpScan.AscanLengthDimensionless)
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

        public Tuple<double, double> CorrectPoint(double v, double t)
        {
            var indV = IndexVelocity(v);
            var indT = IndexTime(t);

            var signMinus = Data[indV][indT] < 0;
            var extremums = signMinus 
                ? FindExtremums(indV, indT, CheckIfMin)
                : FindExtremums(indV, indT, CheckIfMax);

            return GetClosestExtremum(extremums, indV, indT);
        }

        private Tuple<double, double> GetClosestExtremum(List<Tuple<int, int>> extremums, int indV, int indT)
        {
            foreach (var unit in _checkOrderDict)
                if (extremums.Any(x => x.Item1 == (unit.Value.Item1 + indV) && (x.Item2 == unit.Value.Item2 + indT)))
                {
                    var ex = extremums.First(x => x.Item1 == (unit.Value.Item1 + indV) && (x.Item2 == unit.Value.Item2 + indT));
                    return new Tuple<double, double>(
                        Velocity(ex.Item1),
                        Time(ex.Item2));
                }

            return null;
        }

        /// <summary>
        /// Create dictionary of points, according to their closeness to the center
        /// The function of closeness is 1/(x^2+y^2)
        /// </summary>
        private void FillCheckOrderDict()
        {
            var key = 0.0;
            for (int i = 0; i < CheckRadius + 1; i++)
                for (int j = 0; j < CheckRadius + 1; j++)
                {
                    AddToCheckOrderDict(i, -j);
                    AddToCheckOrderDict(-i, j);
                    AddToCheckOrderDict(i, j);
                    AddToCheckOrderDict(-i, -j);
                }

            _checkOrderDict = _checkOrderDict.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
        }

        private void AddToCheckOrderDict(int x, int y)
        {
            var littleVal = 0.00001;
            var key = (x == 0 && y == 0) ? 2 : 1 / Math.Sqrt(x * x + y * y);
            for (; ; )
                if (_checkOrderDict.ContainsKey(key))
                    key -= littleVal;
                else
                    break;
            if (_checkOrderDict.Values.All(t => t.Item1 != x || t.Item2 != y))
                _checkOrderDict.Add(key, new Tuple<int, int>(x, y));
        }
        
        private List<Tuple<int, int>> FindExtremums(int x, int y, Func<int, int, bool> checkFunc)
        {
            var extremums = new List<Tuple<int, int>>();
            
            for (int i = -CheckRadius; i < CheckRadius + 1; i++)
            for (int j = -CheckRadius; j < CheckRadius + 1; j++)
            {
                var x1 = x + i;
                var y1 = y + j;
                if (checkFunc(x1, y1) && !extremums.Any(t => t.Item1 == x1 && t.Item2 == y1))
                    extremums.Add(new Tuple<int, int>(x1, y1));
            }

            return extremums;
        }

        /// <summary>
        /// True - is a case when values of all points around the one in arguments (dist = 1 step) are same or greater than for the one in argument
        /// </summary>
        private bool CheckIf(int x, int y, Func<double, double, bool> condition)
        {
            if (x < 0 || x >= Data.Count || y < 0 || y >= AscanLength)
                return false;
            for (int i = -1; i < 2; i++)
            for (int j = -1; j < 2; j++)
            {
                if (x + i < 0 || x + i >= Data.Count || j + y < 0 || j + y >= AscanLength)
                    return false;
                if (i == 0 && j == 0)
                    continue;
                if (!condition(Data[x][y], Data[x + i][y + j]))
                    return false;
            }

            return true;
        }

        public void AddLayer(double velocity, double time)
        {
            RemoveLayersAround(velocity, time);
            Layers.Add(new Tuple<double, double>(velocity, time));
            Layers.Sort(new TimeComparer());
            RefreshLayers?.Invoke(this, new RefreshLayersEventArgs(Layers));
        }

        public void RemoveLayersAround(double velocity, double time)
        {
            var correctedLayers = new List<Tuple<double, double>>();
            foreach (var point in Layers)
                if (Math.Abs(point.Item2 - time) > MinimalTimeBetweenLayers)
                    correctedLayers.Add(point);

            Layers.Clear();
            foreach (var point in correctedLayers)
                Layers.Add(point);

            RefreshLayers?.Invoke(this, new RefreshLayersEventArgs(Layers));
        }

        private bool CheckIfMax(int x, int y) => CheckIf(x, y, (i, j) => i >= j);

        private bool CheckIfMin(int x, int y) => CheckIf(x, y, (i, j) => i <= j);

        private int IndexVelocity(double velocity) => (int) Math.Round((velocity - MinVelocity) / StepVelocity);

        private int IndexTime(double time) => (int) Math.Round((time - MinTime) / StepTime);

        private double Time(int indexTime) => indexTime * StepTime + MinTime;

        private double Velocity(int indexVelocity) => indexVelocity * StepVelocity + MinVelocity;
    }



    public class TimeComparer : IComparer<Tuple<double, double>>
    {
        public int Compare(Tuple<double, double> x, Tuple<double, double> y)
        {
            return Convert.ToInt32(10 * (x.Item2 - y.Item2));
        }
    }
}