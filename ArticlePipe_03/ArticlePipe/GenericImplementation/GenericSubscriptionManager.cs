using System.Collections.Generic;
using ArticlePipe.AdditionalTypes;
using ArticlePipe.interfaces;

namespace ArticlePipe.GenericImplementation
{
    public sealed class GenericSubscriptionManager<TData>
    {
        private readonly List<ISubscriber<TData>> _subscribers;
        private readonly List<IPublisher<TData>> _publishers;
        private Dictionary<Tag, HashSet<ISubscriber<TData>>> _subscriptions;
        
        #region Constructors

        public GenericSubscriptionManager()
        {
            _subscribers = new List<ISubscriber<TData>>();
            _publishers = new List<IPublisher<TData>>();
            _subscriptions = new Dictionary<Tag, HashSet<ISubscriber<TData>>>();
        }

        #endregion
        
        #region Public

        public void AddPublisher(IPublisher<TData> publisher)
        {
            _publishers.Add(publisher);
            publisher.Created += ArticleProcessor;
        }

        public void AddSubscriber(ISubscriber<TData> subscriber)
        {
            _subscribers.Add(subscriber);

            foreach (var tag in subscriber.Tags)
            {
                var isConstainsTag = _subscriptions.ContainsKey(tag);
                if (isConstainsTag)
                {
                    _subscriptions[tag].Add(subscriber);
                }
            }
        }

        #endregion
        

        #region Private

        private void ArticleProcessor(object sender, IArticle<TData> article)
        {
            foreach (var tag in article.Tags)
            {
                var isConstains = _subscriptions.ContainsKey(tag);
                if (!isConstains)
                {
                    _subscriptions[tag] = new HashSet<ISubscriber<TData>>();
                    UppdateUsersSubscribtions(tag);
                }
            }

            SendNewArticle(article);
        }

        private void UppdateUsersSubscribtions(Tag newTag)
        {
            foreach (var subscriber in _subscribers)
            {
                var isNeededTag = subscriber.Tags.Contains(newTag);
                if (isNeededTag)
                    _subscriptions[newTag].Add(subscriber);
            }
        }

        private void SendNewArticle(IArticle<TData> article)
        {
            var isAlreadySubscribers = new HashSet<ISubscriber<TData>>();

            foreach (var tag in article.Tags)
            {
                var subscribersSet = _subscriptions[tag];

                foreach (var subscriber in subscribersSet)
                {
                    var isFirstArticle = !isAlreadySubscribers.Contains(subscriber);

                    if (!isFirstArticle) continue;
                    subscriber.Receive(article);
                    isAlreadySubscribers.Add(subscriber);
                }

            }
        }
        #endregion
    }
}
