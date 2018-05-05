using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinobytes.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static IList<T> DistinctBy<T, T2>(this IEnumerable<T> items, Func<T, T2> predicate)
        {
            return items.GroupBy(predicate).Select(j => j.FirstOrDefault()).ToList();
        }
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> forEachAction)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (forEachAction == null) throw new ArgumentNullException(nameof(forEachAction));
            foreach (var c in collection) forEachAction(c);
        }
    }
}
