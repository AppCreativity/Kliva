using GalaSoft.MvvmLight.Messaging;
using Kliva.Models;

namespace Kliva.Messages
{
    public class PivotMessage : MessageBase
    {
        public Pivots Pivot { get; set; }
        public bool Visible { get; set; }
        public bool? Show { get; set; }

        public PivotMessage(Pivots pivot, bool visible, bool? show = null)
        {
            Pivot = pivot;
            Visible = visible;
            Show = show;
        }
    }
}
