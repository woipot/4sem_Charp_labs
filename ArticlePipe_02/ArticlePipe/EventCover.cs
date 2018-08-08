using ArticlePipe.realization;
using System;

namespace ArticlePipe
{
    public sealed class EventCover<TData>
    {
        public event EventHandler<Article<TData>> SendEvent;

        public void MakeEvent(object sender, Article<TData> article)
        {
            SendEvent?.Invoke(SendEvent, article);
        }

    }
}
