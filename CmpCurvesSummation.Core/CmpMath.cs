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

        public double Velocity(double permittivity) => SpeedOfLight / (Math.Sqrt(permittivity));
        public double Permittivity(double velocity) => Math.Pow(SpeedOfLight / velocity, 2);
        public double WaterVelocity => Velocity(WaterPermittivity);


        public double Depth(double velocity, double time) => velocity * time / 2;
        
        public double HodographLineClassic(double distance, double height, double velocity)
        {
            var part1 = 1 / velocity;
            var part2 = Math.Sqrt(Math.Pow(height, 2) + Math.Pow(distance / 2, 2));
            return part1 * part2;
        }

        public double HodographLineLoza(double distance, double height, double velocity)
        {
            double part1 = 1 / velocity;
            double part2 = Math.Sqrt(Math.Pow(height * 2, 2) + Math.Pow(distance, 2));
            var part3 = distance / SpeedOfLight;
            return part1 * part2 - part3;
        }
        
        public double LayerThickness(double depth, double depthPrevious)
        {
            return depth - depthPrevious;
        }

        public double LayerVelocity(double time, double distance, double depth, double velocity, 
            double depthPrevious, double velocityPrevious)
        {
            time = time + distance / SpeedOfLight;

            var partT1 = time;
            var partT2 = velocity / velocityPrevious;
            var partT3 = depthPrevious / depth;
            var timeLayer =  time - partT1 * partT2 * partT3;

            var partV1 = velocity;
            var partV2 = (depth - depthPrevious) / depth;
            var partV3 = time / timeLayer;

            var velocityLayer = partV1 * partV2 * partV3;
            return velocityLayer;
        }



    }
}