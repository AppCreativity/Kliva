using System.Collections.Generic;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Messaging;

namespace Kliva.Messages
{
    public class PolylineMessage : MessageBase
    {
        public List<BasicGeoposition> Geopositions { get; private set; }

        public PolylineMessage(List<BasicGeoposition> geopositions)
        {
            Geopositions = geopositions;
        }        
    }
}
