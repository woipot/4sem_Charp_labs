using System.Collections.Generic;
using ArticlePipe.AdditionalTypes;

namespace ArticlePipe.interfaces
{
    public interface ISubscriber<in TData>
    {
        HashSet<Tag> Tags
        {
            get;
        }

        void Receive(IArticle<TData> genericArticle);

    }
}
