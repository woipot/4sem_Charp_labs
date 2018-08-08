using System.Collections.Generic;
using System.Linq;

namespace ArticlePipe.Extensions
{
    public static class IReadOnlyListExtensions
    {
        public static bool ContainsAny<TData>(this IReadOnlyList<TData> first, IReadOnlyList<TData> second)
        {
            foreach (var token in first)
            {
                var isMatch = second.Contains(token);
                if (isMatch)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
