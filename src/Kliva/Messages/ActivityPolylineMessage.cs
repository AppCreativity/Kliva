using System.Collections.Generic;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Messaging;

namespace Kliva.Messages
{
    public class ActivityPolylineMessage : MessageBase
    {
        public List<BasicGeoposition> Geopositions { get; private set; }

        public ActivityPolylineMessage(List<BasicGeoposition> geopositions)
        {
            this.Geopositions = geopositions;
        }        
    }
}
