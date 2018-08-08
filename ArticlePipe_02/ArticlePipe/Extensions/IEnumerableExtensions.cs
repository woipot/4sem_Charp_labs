using System.Collections.Generic;

namespace ArticlePipe.Extensions
{
    public static class IEnumerableExtensions
    {
        public static HashSet<TData> ToHashSet<TData>(this IEnumerable<TData> first)
        {
            var newHashSet = new HashSet<TData>();
            foreach (var token in first)
            {
                newHashSet.Add(token);
            }
            return newHashSet;
        }
    }
}
