using Kliva.Converters;

namespace Kliva.Helpers
{
    public static class Converters
    {
        public static DistanceUnitToStringConverter DistanceConverter => new DistanceUnitToStringConverter();
        public static SpeedUnitToStringConverter SpeedConverter => new SpeedUnitToStringConverter();
        public static SecondsToTimeStringConverter SecToTimeConverter => new SecondsToTimeStringConverter();
    }
}
