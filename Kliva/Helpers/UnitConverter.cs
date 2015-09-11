using Kliva.Models;

namespace Kliva.Helpers
{
    public enum SpeedUnit
    {
        MetresPerSecond,
        KilometresPerHour,
        MilesPerHour
    }

    public static class UnitConverter
    {
        public static float[] DistanceConversions =
        {
            0.001F,             //Kilometre
            0.00062137119224F,  //Miles
            1F,                 //Metre
            3.2808399F          //Feet
        };

        public static string[] DistanceSymbols =
        {
            "m",
            "ft",
            "km",
            "mi"
        };

        public static float[] SpeedConversions =
        {
            1F,                 //Metre per second
            3.6F,               //Kilometre per hour
            2.236936292064F     //Miles per hour
        };

        public static string[] SpeedSymbols =
        {
            "m/s",
            "km/h",
            "mi/h"
        };

        public static float ConvertDistance(float value, DistanceUnitType fromUnit, DistanceUnitType toUnit)
        {
            float workingValue;

            if (fromUnit == DistanceUnitType.Metres)
                workingValue = value;
            else
                workingValue = value / DistanceConversions[(int)fromUnit];

            if (toUnit != DistanceUnitType.Metres)
                workingValue = workingValue * DistanceConversions[(int)toUnit];

            return workingValue;
        }

        public static float ConvertSpeed(float value, SpeedUnit fromUnit, SpeedUnit toUnit)
        {
            float workingValue;

            if (fromUnit == SpeedUnit.MetresPerSecond)
                workingValue = value;
            else
                workingValue = value / SpeedConversions[(int)fromUnit];

            if (toUnit != SpeedUnit.MetresPerSecond)
                workingValue = workingValue * SpeedConversions[(int)toUnit];

            return workingValue;
        }
    }
}