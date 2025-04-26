using System.Collections.Generic;
using System.Linq;

public static class CollectionUtils
{
    public static Dictionary<K, V> Copy<K, V>(this IReadOnlyDictionary<K, V> dictionary) => dictionary.ToDictionary(pair => pair.Key, pair => pair.Value);
}