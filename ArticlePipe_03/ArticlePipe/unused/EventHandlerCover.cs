using System;

namespace ArticlePipe
{
    public sealed class EventHandlerCover<TData>
    {
        public EventHandler<TData> TDataEvent;


        public void MakeEvent(TData tdata, object sender = null)
        {
            TDataEvent?.Invoke(sender, tdata);
        }
    }
}
