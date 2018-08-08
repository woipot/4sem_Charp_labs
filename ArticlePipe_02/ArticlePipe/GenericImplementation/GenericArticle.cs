using System.Collections.Generic;
using ArticlePipe.AdditionalTypes;
using ArticlePipe.Extensions;
using ArticlePipe.interfaces;

namespace ArticlePipe.GenericImplementation
{
    public sealed class GenericArticle<TData> : IArticle<TData>
    { 
        private readonly TData _data;
        private readonly HashSet<Tag> _tags;

        
        #region Constructors

        public GenericArticle(IEnumerable<Tag> tags, TData data)
        {
            _data = data;
            _tags = tags.ToHashSet();
        }

        #endregion
        

        #region Properties
        public TData Data => _data;
        public HashSet<Tag> Tags => _tags;

        #endregion

        public TData GetArticle()
        {
            return Data;
        }
    }
}
