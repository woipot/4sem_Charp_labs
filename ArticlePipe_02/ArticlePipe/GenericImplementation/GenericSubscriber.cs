using System;
using System.Collections.Generic;
using ArticlePipe.AdditionalTypes;
using ArticlePipe.interfaces;

namespace ArticlePipe.GenericImplementation
{
    public sealed class GenericSubscriber<TData> : ISubscriber<TData>
    {
        private readonly string _name;

        private readonly HashSet<Tag> _tags;

        #region Constructors 
        public GenericSubscriber(string name, HashSet<Tag> tags)
        {

            _tags = tags;
            _name = name;
        }

        #endregion

        public string Name => _name;
        

        #region ISubscriber

        public HashSet<Tag> Tags => _tags;


        public void Receive(IArticle<TData> genericArticle)
        {
            Console.WriteLine($"--------------------------Hello i am {_name}\n" + 
                              $"Tags: " + string.Join(" ", genericArticle.Tags) + 
                              "\n" + genericArticle.GetArticle() + "\n");
        }

        #endregion
    }
}
