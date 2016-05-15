using GalaSoft.MvvmLight.Messaging;
using Kliva.Models;

namespace Kliva.Messages
{
    public class SegmentEffortMessage : MessageBase
    {
        public SegmentEffort SegmentEffort { get; private set; }

        public SegmentEffortMessage(SegmentEffort segmentEffort)
        {
            SegmentEffort = segmentEffort;
        }
    }
}
