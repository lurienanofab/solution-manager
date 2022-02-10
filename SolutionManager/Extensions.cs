using System;
using System.Collections.Generic;
using System.Linq;

namespace SolutionManager;

public static class Extensions
{
    public static TItem[] OrdinalSort<TItem, TKey>(this IEnumerable<TItem> source, Func<TItem, TKey> keySelector)
    {
        var keys = source.Select(keySelector).ToArray();
        var items = source.ToArray();
        Array.Sort(keys, items, StringComparer.Ordinal);
        return items;
    }
}