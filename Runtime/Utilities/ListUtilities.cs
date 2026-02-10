using System.Collections.Generic;
using UnityEngine;

public static class ListUtilities
{
    /// <summary>
    /// Returns a random item from the list items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns>A random element of type <typeparamref name="T" />.</returns>
    public static T GetRandomItem<T>(this IList<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Shuffles the list using Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T value = list[i];
            list[i] = list[j];
            list[j] = value;
        }
    }
}
