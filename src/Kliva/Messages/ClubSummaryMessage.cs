using GalaSoft.MvvmLight.Messaging;
using Kliva.Models;

namespace Kliva.Messages
{
    public class ClubSummaryMessage : MessageBase
    {
        public ClubSummary Club { get; private set; }

        public ClubSummaryMessage(ClubSummary club)
        {
            Club = club;
        }
    }
}
