using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Krista.FM.Extensions
{
    public static class DictionaryExtensions
    {
        [DebuggerStepThrough]
        public static TValue ValueOfDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                          TKey key,
                                                          TValue defaultValue)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
        }

        [DebuggerStepThrough]
        public static TValue ValueOfDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                          TKey key)
        {
            return dictionary.ValueOfDefault(key, default(TValue));
        }
    }
}
