using GalaSoft.MvvmLight.Messaging;

namespace Kliva.Messages
{
    public class PivotMessage<T> : MessageBase
    {
        public T Pivot { get; set; }
        public bool Visible { get; set; }
        public bool? Show { get; set; }

        public PivotMessage(T pivot, bool visible, bool? show = null)
        {
            Pivot = pivot;
            Visible = visible;
            Show = show;
        }
    }
}
