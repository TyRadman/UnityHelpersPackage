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
    public static T RandomItem<T>(this IList<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Returns a random item from the list and optionally removes it.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to select a random item from.</param>
    /// <param name="removeItem">If true, removes the selected item from the list.</param>
    /// <returns>A random item from the list.</returns>
    public static T RandomItem<T>(this IList<T> list, bool removeItem)
    {
        T selectedItem = list[Random.Range(0, list.Count)];

        if (removeItem)
        {
            list.Remove(selectedItem);
        }

        return selectedItem;
    }

    /// <summary>
    /// Creates a duplicate of the list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to duplicate.</param>
    /// <returns>A new list containing the same elements as the original list.</returns>
    public static List<T> Duplicate<T>(this List<T> list)
    {
        List<T> newList = new List<T>();
        list.ForEach(i => newList.Add(i));
        return newList;
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

    /// <summary>
    /// Returns false if the list is empty or uninitialized.
    /// </summary>
    /// <param name="list"></param>
    public static bool IsEmpty<T>(this IList<T> list)
    {
        return list == null || list.Count == 0;
    }
}
