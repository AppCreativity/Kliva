using Kliva.Models;

namespace Kliva.Helpers
{
    public static class MeasurementHelper
    {
        public static bool IsMetric(DistanceUnitType distanceUnitType)
        {
            return distanceUnitType == DistanceUnitType.Kilometres || distanceUnitType == DistanceUnitType.Metres;
        }

        public static DistanceUnitType GetElevationUnitType(bool isMetric)
        {
            return isMetric ? DistanceUnitType.Metres : DistanceUnitType.Feet;
        }

        public static DistanceUnitType GetDistanceUnitType(bool isMetric)
        {
            return isMetric ? DistanceUnitType.Kilometres : DistanceUnitType.Miles;
        }

        public static SpeedUnit GetSpeedUnit(bool isMetric)
        {
            return isMetric ? SpeedUnit.KilometresPerHour : SpeedUnit.MilesPerHour;
        }
    }
}
