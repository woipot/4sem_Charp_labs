using System.Collections.Generic;
using ArticlePipe.AdditionalTypes;

namespace ArticlePipe.interfaces
{
    public interface IArticle<out TReturnType>
    {
        HashSet<Tag> Tags  {get;}

        TReturnType GetArticle();
    }
}
