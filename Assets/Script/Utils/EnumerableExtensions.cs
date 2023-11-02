using System;
using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static T GetRandomElement<T>(this IList<T> ts, Predicate<T> predicate = null)
    {
        T element = default;
        int counter = 0;

        if (predicate != null)
        {
            while (counter < 100)
            {
                counter++;

                element = ts[UnityEngine.Random.Range(0, ts.Count)];

                if (predicate(element))
                    return element;
            }
        }

        return ts[UnityEngine.Random.Range(0, ts.Count)];
    }

    public static T GetRandomElement<T>(this T[] ts, Predicate<T> predicate = null)
    {
        T element = default;
        int counter = 0;

        if (predicate != null)
        {
            while (counter < 100)
            {
                counter++;

                element = ts[UnityEngine.Random.Range(0, ts.Length)];

                if (predicate(element))
                    return element;
            }
        }

        return ts[UnityEngine.Random.Range(0, ts.Length)];
    }
}