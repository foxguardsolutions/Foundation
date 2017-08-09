using System.Collections.Generic;
using System.Linq;

namespace FGS.Pump.Extensions
{
    public static class CollectionExtensions
    {
        public static ICollection<T> TakeLast<T>(this ICollection<T> collection, int count)
        {
            return collection.Reverse().Take(count).Reverse().ToList();
        }
    }
}