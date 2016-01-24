using System.Collections.Generic;

namespace Clapton.Extensions
{
    /// <summary>
    /// Provides extensions for Dictionaries
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains all specified keys.
        /// </summary>
        /// <param name="values">The keys to locate in <see cref="Dictionary{TKey, TValue}"/></param>
        public static bool ContainsKey<TKey,TValue>(this Dictionary<TKey, TValue> obj, TKey[] values)
        {
            foreach (TKey value in values)
                if (!obj.ContainsKey(value))
                    return false;
            return true;
        }

        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains all specified values.
        /// </summary>
        /// <param name="values">The values to locate in <see cref="Dictionary{TKey, TValue}"/></param>
        public static bool ContainsValue<TKey,TValue>(this Dictionary<TKey, TValue> obj, TValue[] values)
        {
            foreach (TValue value in values)
                if (!obj.ContainsValue(value))
                    return false;
            return true;
        }
    }
}
