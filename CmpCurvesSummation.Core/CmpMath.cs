using System;

namespace CmpCurvesSummation.Core
{
    public class CmpMath
    {
        private static Lazy<CmpMath> _instance = new Lazy<CmpMath>(() => new CmpMath());
        public static CmpMath Instance => _instance.Value;


        public const double SpeedOfLight = 0.3;

        public double Permittivity(double velocity) => Math.Pow(SpeedOfLight / 2 / velocity, 2);


        public double HodographLine(double distance, double height, double velocity)
        {
            var part1 = 1 / velocity;
            var part2 = Math.Sqrt(Math.Pow(height, 2) + Math.Pow(distance / 2, 2));
            return part1 * part2;
        }

        public double HodographLineLoza(double distance, double height, double velocity)
        {
            var part1 = 1 / velocity;
            var part2 = Math.Sqrt(Math.Pow(height, 2) + Math.Pow(distance / 2, 2));
            return part1 * part2 - distance / SpeedOfLight;
        }



    }
}