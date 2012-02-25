namespace WellCompareDir.WPF.Library.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class IEnumerableExtensions
    {
        public static T FilterOnlyOrDefault<T>(ref IEnumerable<T> collection, Predicate<T> predicate)
        {
            collection = collection.Where(item => predicate(item));

            return collection.SingleOrDefault();
        }
    }
}
