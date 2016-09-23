using GalaSoft.MvvmLight.Messaging;
using Kliva.Models;

namespace Kliva.Messages
{
    public class MeasureUnitChangedMessage : MessageBase
    {
        public DistanceUnitType NewValue { get; private set; }

        public MeasureUnitChangedMessage(DistanceUnitType newValue)
        {
            NewValue = newValue;
        }
    }
}
