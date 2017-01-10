//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AdvantShop
{
    /// <summary>
    /// Summary description for Generic
    /// </summary>
    public static class Generic
    {
        public static IDictionary<TKey, TVal> AddRange<TKey, TVal>(this IDictionary<TKey, TVal> dict, IEnumerable<KeyValuePair<TKey, TVal>> values)
        {
            foreach (var kvp in values)
            {
                if (dict.ContainsKey(kvp.Key))
                {
                    throw new ArgumentException("An item with the same key has already been added.");
                }
                dict.Add(kvp);
            }
            return dict;
        }
        public static TValue ElementOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.ElementOrDefault(key, default(TValue));
        }
        public static TValue ElementOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
        }
        public static string AggregateString<TSource>(this IEnumerable<TSource> values)
        {
            return values.Aggregate(new StringBuilder(), (curr, val) => curr.Append(val.ToString())).ToString();
        }

        public static string AggregateString<TSource>(this IEnumerable<TSource> values, char separator)
        {
            return values.Aggregate(new StringBuilder(), (curr, val) => curr.Append(val.ToString()).Append(separator), curr => curr.ToString().TrimEnd(separator));
        }

        public static string AggregateString<TSource>(this IEnumerable<TSource> values, string separator)
        {
            return values.Aggregate(new StringBuilder(), (curr, val) => curr.Append(val.ToString()).Append(separator), curr => curr.ToString().TrimEnd(separator.ToCharArray()));
        }

        public static string AggregateString<TSource>(this IEnumerable<TSource> values, string seed, string end)
        {
            return values.Aggregate(new StringBuilder(seed), (curr, val) => curr.Append(val.ToString()), curr => curr.Append(end)).ToString();
        }
        public static string AggregateString<TSource>(this IEnumerable<TSource> values, string seed, string end, char separator)
        {
            return values.Aggregate(new StringBuilder(seed), (curr, val) => curr.Append(val.ToString()).Append(separator), curr => curr.ToString().TrimEnd(separator) + end);
        }

        public static TResult WithId<TResult>(this IEnumerable<TResult> list, int id) where TResult : IDentable
        {
            return list.FirstOrDefault(item => item.ID == id);
        }

        public static IEnumerable<TResult> AllWithId<TResult>(this IEnumerable<TResult> list, int id) where TResult : IDentable
        {
            return list.Where(item => item.ID == id);
        }

        public static IEnumerable<TResult> WithIDs<TResult>(this IEnumerable<TResult> src, IEnumerable<int> ids) where TResult : IDentable
        {
            return src.Where(item => ids.Contains(item.ID));
        }

        public static IEnumerable<TResult> WithOutIDs<TResult>(this IEnumerable<TResult> src, IEnumerable<int> ids) where TResult : IDentable
        {
            return src.Where(item => !ids.Contains(item.ID));
        }
        public static IEnumerable<TResult> Except<TResult, TExcept>(this IEnumerable<TResult> src, IEnumerable<TExcept> except, Func<TResult, TExcept, bool> comparer)
        {
            if (except == null)
                return src;
            if (comparer == null)
                return
                    (typeof(TExcept).Equals(typeof(TResult)))
                        ? src.Except(except.Cast<TResult>())
                        : src;
            return src.Where(item => !except.Any(exceptItem => comparer(item, exceptItem)));
        }
        public static IEnumerable<TResult> Intersect<TResult, TExcept>(this IEnumerable<TResult> src, IEnumerable<TExcept> except, Func<TResult, TExcept, bool> comparer)
        {
            if (except == null)
                return src;
            if (comparer == null)
                return
                    (typeof(TExcept).Equals(typeof(TResult)))
                        ? src.Intersect(except.Cast<TResult>())
                        : src;
            return src.Where(item => except.Any(exceptItem => comparer(item, exceptItem)));
        }
        public static void ForEach<T>(this IEnumerable<T> iEnumerable, Action<T> func)
        {
            foreach (T val in iEnumerable)
            {
                func(val);
            }
        }
        public static int AggregateHash<T>(this IEnumerable<T> val)
        {
            return val.Aggregate(0, (curr, seed) => curr ^ seed.GetHashCode());
        }

        public static bool HasDuplicates<T>(this IEnumerable<T> subjects)
        {
            return HasDuplicates(subjects, EqualityComparer<T>.Default);
        }

        public static bool HasDuplicates<T>(this IEnumerable<T> subjects, IEqualityComparer<T> comparer)
        {
            if (subjects == null)
                throw new ArgumentNullException("subjects");

            if (comparer == null)
                throw new ArgumentNullException("comparer");

            var set = new HashSet<T>(comparer);

            foreach (var s in subjects)
                if (!set.Add(s))
                    return true;

            return false;
        }

        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> subjects)
        {
            return Duplicates(subjects, EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> subjects, IEqualityComparer<T> comparer)
        {
            if (subjects == null)
                throw new ArgumentNullException("subjects");

            if (comparer == null)
                throw new ArgumentNullException("comparer");

            var set = new HashSet<T>(comparer);
            return subjects.Where(x => !set.Add(x));
        }

        /// <summary>
        /// Deep copy object to new instant
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static bool IsDefault<T>(this T value) where T : struct
        {
            var isDefault = value.Equals(default(T));
            return isDefault;
        }
    }
}