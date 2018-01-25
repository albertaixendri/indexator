using System;
using System.Collections.Generic;
using System.Linq;

namespace Indexator.Utils
{
    public static class IEnumerableUtils
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        public static IEnumerable<T> DoForEach<T>(this IEnumerable<T> data, Action<T> action)
        {
            foreach (T value in data)
            {
                action(value);
                yield return value;
            }
        }

        public static IEnumerable<T> DoForAll<T>(this IEnumerable<T> data, Action<IEnumerable<T>> action)
        {
            action(data);
            return data;
        }

        public static void Do<T>(this IEnumerable<T> data)
        {
            foreach (T value in data)
            {
            }
        }

        public static IEnumerable<T> DoForEach<T>(this IOrderedEnumerable<T> data, Action<T> action)
        {
            foreach (T value in data)
            {
                action(value);
                yield return value;
            }
        }

        public static IEnumerable<T> Intersect<T>(this IEnumerable<IEnumerable<T>> enumerables)
        {
            IEnumerable<T> intersection = null;
            foreach(IEnumerable<T> enumerable in enumerables)
            {
                if (enumerable == null)
                    return null;
                if (intersection == null)
                {
                    intersection = enumerable;
                } 
                else
                {
                    intersection = intersection.Intersect(enumerable);
                }

                if (intersection.Count() == 0)
                    return intersection;
            }

            return intersection;
        }
    }
}