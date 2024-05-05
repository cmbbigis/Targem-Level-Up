using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Common
{
    public static class SerializedDictionaryExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this SerializedDictionary<TKey, TValue> dict)
        {
            var res = new Dictionary<TKey, TValue>();
            foreach (var pair in dict)
                res[pair.Key] = pair.Value;
            return res;
        }
    }
}