using System;

namespace CmpCurvesSummation.Core
{
    /// <summary>
    /// Different math for GPR calculations
    /// </summary>
    public class CmpMath
    {
        private static Lazy<CmpMath> _instance = new Lazy<CmpMath>(() => new CmpMath());
        public static CmpMath Instance => _instance.Value;


        public const double SpeedOfLight = 0.3;
        public const double WaterPermittivity = 81;

        public double Velocity(double permittivity) => SpeedOfLight / (2 * Math.Sqrt(permittivity));
        public double Permittivity(double velocity) => Math.Pow(SpeedOfLight / 2 / velocity, 2);
        public double WaterVelocity => Velocity(WaterPermittivity);


        public double HodographLineClassic(double distance, double height, double velocity)
        {
            var part1 = 1 / velocity;
            var part2 = Math.Sqrt(Math.Pow(height, 2) + Math.Pow(distance / 2, 2));
            return part1 * part2;
        }

        public double HodographLineLoza(double distance, double height, double velocity)
        {
            double part1 = 1 / velocity;
            double part2 = Math.Sqrt(Math.Pow(height, 2) + Math.Pow(distance / 2, 2));
            var part3 = distance / SpeedOfLight;
            return part1 * part2 - part3;
        }

        public double LayerThickness(double velocity, double timeCurrentDistanceZero, double timePreviousDistanceZero = 0)
        {
            return velocity * (timeCurrentDistanceZero - timePreviousDistanceZero);
        }

        public double LayerVelocity(double timeCurrentDistanceZero, double timePreviousDistanceZero, double avgVelocityCurrent, double avgVelocityPrevious)
        {
            var part1 = timeCurrentDistanceZero * avgVelocityCurrent * avgVelocityCurrent - timePreviousDistanceZero * avgVelocityPrevious * avgVelocityPrevious;
            var part2 = timeCurrentDistanceZero - timePreviousDistanceZero;
            return part1 / part2;
        }

    }
}