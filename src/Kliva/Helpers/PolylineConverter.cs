using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;

namespace Kliva.Helpers
{
    public static class PolylineConverter
    {
        public static List<BasicGeoposition> DecodePolylinePoints(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints)) return null;

            var poly = new List<BasicGeoposition>();
            var polylinechars = encodedPoints.ToCharArray();
            var index = 0;

            var currentLat = 0;
            var currentLng = 0;

            try
            {
                while (index < polylinechars.Length)
                {
                    //calculate next latitude
                    var sum = 0;
                    var shifter = 0;
                    int next5Bits;
                    do
                    {
                        next5Bits = (int)polylinechars[index++] - 63;
                        sum |= (next5Bits & 31) << shifter;
                        shifter += 5;
                    } while (next5Bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5Bits = polylinechars[index++] - 63;
                        sum |= (next5Bits & 31) << shifter;
                        shifter += 5;
                    } while (next5Bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5Bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    var p = new BasicGeoposition { Latitude = Convert.ToDouble(currentLat) / 100000.0, Longitude = Convert.ToDouble(currentLng) / 100000.0 };
                    poly.Add(p);
                }
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Log exception
            }

            return poly;
        }
    }
}
