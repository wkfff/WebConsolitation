using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Krista.FM.Extensions
{
    public static class EnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                action(item);
            }
        }

        public static void Each<T>(this IEnumerable items, Action<T> action)
        {
            foreach (object item in items)
            {
                action((T) item);
            }
        }

        /// <summary>
        /// Суммирует элементы коллекции. Пустые элементы интерпритируются как 0.
        /// </summary>
        /// <typeparam name="TSource">Тип исходного объекта.</typeparam>
        /// <param name="source">Коллекция элементов для суммирования.</param>
        /// <param name="selector">Селектор значения.</param>
        public static decimal? SumWithNull<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            return source.Select(selector).SumWithNull();
        }

        /// <summary>
        /// Суммирует элементы коллекции. Пустые элементы интерпритируются как 0.
        /// </summary>
        /// <param name="source">Коллекция элементов для суммирования.</param>
        public static decimal? SumWithNull(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            decimal? num = null;
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    if (!num.HasValue)
                    {
                        num = 0;
                    }

                    num += nullable.GetValueOrDefault();
                }
            }
            return num;
        }

        /// <summary>
        /// Суммирует элементы коллекции. Пустые элементы интерпритируются как 0.
        /// </summary>
        /// <typeparam name="TSource">Тип исходного объекта.</typeparam>
        /// <param name="source">Коллекция элементов для суммирования.</param>
        /// <param name="selector">Селектор значения.</param>
        public static int? SumWithNull<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            return source.Select(selector).SumWithNull();
        }

        /// <summary>
        /// Суммирует элементы коллекции. Пустые элементы интерпритируются как 0.
        /// </summary>
        /// <param name="source">Коллекция элементов для суммирования.</param>
        public static int? SumWithNull(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            int? num = null;
            foreach (var nullable in source)
            {
                if (nullable.HasValue)
                {
                    if (!num.HasValue)
                    {
                        num = 0;
                    }

                    num += nullable.GetValueOrDefault();
                }
            }
            return num;
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            return ZipIterator(first, second, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(
            IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> iteratorVariable0 = first.GetEnumerator())
            {
                using (IEnumerator<TSecond> iteratorVariable1 = second.GetEnumerator())
                {
                    while (iteratorVariable0.MoveNext() && iteratorVariable1.MoveNext())
                    {
                        yield return resultSelector(
                            iteratorVariable0.Current,
                            iteratorVariable1.Current);
                    }
                }
            }
        }

        [DebuggerStepThrough]
        public static void Each<T>(this IEnumerable<T> values, Action<T, int> eachAction)
        {
            values
                .Select((vals, i) => new {Values = vals, Index = i})
                .Each(x => eachAction(x.Values, x.Index));
        }
    }
}
