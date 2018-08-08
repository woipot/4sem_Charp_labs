using System;

namespace ArticlePipe.interfaces
{
    public interface IPublisher<TArticleData>
    {
        event EventHandler<IArticle<TArticleData>> Created;
    }
}
