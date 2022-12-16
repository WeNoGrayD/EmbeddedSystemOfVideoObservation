using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedSystemBigBrother
{
    internal static class DictionaryCastingHelper
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> protoDict)
        {
            return protoDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
