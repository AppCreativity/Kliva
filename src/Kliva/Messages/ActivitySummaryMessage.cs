using GalaSoft.MvvmLight.Messaging;
using Kliva.Models;

namespace Kliva.Messages
{
    public class ActivitySummaryMessage : MessageBase
    {
        public ActivitySummary ActivitySummary { get; private set; }

        public ActivitySummaryMessage(ActivitySummary activitySummary)
        {
            ActivitySummary = activitySummary;
        }
    }
}
