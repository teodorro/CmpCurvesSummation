using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        int CheckRadius { get; set; }
        List<Tuple<double, double>> Layers { get; }

        double[,] GetDataArray();
        void Sum(ICmpScan cmpScan);
        Tuple<double, double> CorrectPoint(double velocity, double time);
        void AddLayer(double velocity, double time);
        void RemoveLayersAround(double velocity, double time);
        void RemoveRightAscans(double maxVelocity);
        void Power(double powered);

        event RefreshLayersHandler RefreshLayers;
    }



    public class SummedScanVT : ISummedScanVT
    {
        private const int _vLengthDimensionless = 300;
        public const double AbsoluteMinVelocity = 0.015;

        private Dictionary<double, Tuple<int, int>> _checkOrderDict = new Dictionary<double, Tuple<int, int>>();

        public List<double[]> Data { get; } = new List<double[]>();
        public List<double[]> RawData { get; } = new List<double[]>();
        public double StepVelocity => (LightRadarVelocity - MinVelocity) / _vLengthDimensionless;
        public double StepTime { get; }
        public double StepDistance { get; }
        public double MinVelocity { get; } = AbsoluteMinVelocity; //CmpMath.Instance.Velocity(CmpMath.WaterPermittivity) / 2;
        public double LightRadarVelocity { get; } = CmpMath.SpeedOfLight / 2;
        public double MaxVelocity { get; private set; } = CmpMath.SpeedOfLight / 3;
        public double MinTime { get; } = 0;
        public double MaxTime => MinTime + AscanLength;
        public int AscanLengthDimensionless { get; }
        public double AscanLength => AscanLengthDimensionless * StepTime;
        public int CheckRadius { get; set; } = 5;
        public List<Tuple<double, double>> Layers { get; } = new List<Tuple<double, double>>();

        public event RefreshLayersHandler RefreshLayers;


        public SummedScanVT(ICmpScan cmpScan, double maxVelocity)
        {
            if (maxVelocity > LightRadarVelocity || maxVelocity < MinVelocity)
                throw new ArgumentOutOfRangeException("Недопустимая максимальная скорость");
            MaxVelocity = maxVelocity;
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
            var vStep = (LightRadarVelocity - MinVelocity) / _vLengthDimensionless;

            for (int p = 0; p < _vLengthDimensionless; p++)
            {
                CmpProgressBar.Instance.ProgressValue = p;
                v = vStep * p + MinVelocity;
                RawData.Add(new double[AscanLengthDimensionless]);
                for (int j = 0; j < AscanLengthDimensionless; j++)
                {
                    var t = Time(j);
                    h = CmpMath.Instance.Depth(v, t);
                    var sum = CalcSumForVelocityAndDepth(cmpScan, h, v);
                    //                    Data[p][j] = Math.Sign(sum) * Math.Log(Math.Abs(sum) + 1); // log for sum works not good
                    //                    Data[p][j] = Math.Sign(sum) * Math.Sqrt(Math.Abs(sum));
                    RawData[p][j] = sum;
                }
            }

            CopyRawToActual();
        }

        private void CopyRawToActual()
        {
            Data.Clear();
            for (int i = 0; i < RawData.Count; i++)
            {
                Data.Add(new double[RawData[i].Length]);

                for (int j = 0; j < RawData[i].Length; j++)
                    Data[i][j] = RawData[i][j];
            }
        }

        public void RemoveRightAscans(double maxVelocity)
        {
            if (maxVelocity > LightRadarVelocity || maxVelocity < MinVelocity)
                throw new ArgumentOutOfRangeException("Недопустимая максимальная скорость");
            MaxVelocity = maxVelocity;
            var indMaxVelocity = (int)Math.Round((maxVelocity - MinVelocity) / StepVelocity);
            CopyRawToActual();
            Data.RemoveRange(indMaxVelocity, RawData.Count - indMaxVelocity);
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
            var res = new double[Data.Count, AscanLengthDimensionless];

            for (int i = 0; i < Data.Count; i++)
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
        
        public List<Tuple<int, int>> FindExtremums(int x, int y, Func<int, int, bool> checkFunc)
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
        /// True - is a case when values of all points around the one in arguments (dist = 1 step) are 'condition' than for the one in argument
        /// </summary>
        public bool CheckIf(int x, int y, Func<double, double, bool> condition)
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
                if (Math.Abs(point.Item2 - time) > CheckRadius)
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

        public void Power(double powered)
        {
            RemoveRightAscans(MaxVelocity);
            foreach (var ascan in Data)
            {
                for (int i = 0; i < ascan.Length; i++)
                    ascan[i] = Math.Sign(ascan[i]) * Math.Pow(Math.Abs(ascan[i]), powered);
            }
        }


    }



    public class TimeComparer : IComparer<Tuple<double, double>>
    {
        public int Compare(Tuple<double, double> x, Tuple<double, double> y)
        {
            return Convert.ToInt32(10 * (x.Item2 - y.Item2));
        }
    }
}