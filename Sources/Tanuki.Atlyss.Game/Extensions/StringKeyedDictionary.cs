using System;
using System.Collections.Generic;

namespace Tanuki.Atlyss.Game.Extensions;

public static class StringKeyedDictionary
{
    extension<T>(IDictionary<string, T> instance)
    {
        public bool TryGetValueByExactKey(string key, out T value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (KeyValuePair<string, T> keyValuePair in instance)
            {
                if (!string.Equals(key, keyValuePair.Key, stringComparison))
                    continue;

                value = keyValuePair.Value;
                return true;
            }

            value = default!;
            return false;
        }

        public bool TryGetValueBySimilarKey(string key, out T value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (KeyValuePair<string, T> keyValuePair in instance)
            {
                if (keyValuePair.Key.IndexOf(key, stringComparison) < 0)
                    continue;

                value = keyValuePair.Value;
                return true;
            }

            value = default!;
            return false;
        }

        public bool TryGetValueFlexible(
            string key,
            out T value,
            bool exactMatch = false,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase
        ) =>
            instance.TryGetValue(key, out value) ||
            (
                exactMatch ?
                    instance.TryGetValueByExactKey(key, out value, stringComparison)
                    :
                    instance.TryGetValueBySimilarKey(key, out value, stringComparison)
            );
    }
}
