using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class StringUtilities
{
    /// <summary>
    /// Wraps the string in a Unity rich text color tag using the specified color.
    /// </summary>
    /// <param name="value">The text to color.</param>
    /// <param name="color">The color applied to the text.</param>
    /// <returns>The string wrapped in a color tag.</returns>
    public static string Color(this string value, Color color)
    {
        return $"<color=#{color.ToHex()}>{value}</color>";
    }

    /// <summary>
    /// Wraps the string in a Unity rich text bold tag.
    /// </summary>
    /// <param name="value">The text to make bold.</param>
    /// <returns>The string wrapped in a bold tag.</returns>
    public static string Bold(this string value)
    {
        return $"<b>{value}</b>";
    }

    /// <summary>
    /// Wraps the string in a monospace spacing tag with the given em value.
    /// </summary>
    /// <param name="value">The text to format.</param>
    /// <param name="em">The em spacing value applied to the text.</param>
    /// <returns>The formatted string using a monospace spacing tag.</returns>
    public static string Mono(this string value, float em)
    {
        return $"<mspace em={em}>{value}</mspace>";
    }

    /// <summary>
    /// Prefixes the string value with the full name of the object type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="titleObject">The object that the type of will be added as a prefix. Almost always pass 'this'.</param>
    /// <returns>The prefixed string value.</returns>
    public static string Class(this string value, object titleObject)
    {
        return $"[{titleObject.GetType().FullName}] {value}";
    }

    /// <summary>
    /// Prefixes the string value with the time since the start of the app i.e. Time.realtimeSinceStartup.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The newly generated string that has the time prefixed to the original message.</returns>
    public static string AddTime(this string value)
    {
        return $"[{Time.realtimeSinceStartup}] {value}";
    }

    /// <summary>
    /// Converts a list into a comma-separated string using a custom converter.
    /// </summary>
    /// <typeparam name="T">Type of the elements in the list.</typeparam>
    /// <param name="list">The list to convert.</param>
    /// <param name="converter">Function that converts each element to a string.</param>
    /// <returns>A comma-separated string representing the list.</returns>
    public static string EntriesToString<T>(this IList<T> list, Func<T, string> converter)
    {
        StringBuilder message = new StringBuilder();

        for (int i = 0; i < list.Count; i++)
        {
            if(i > 0)
            {
                message.Append(", ");
            }

            message.Append(converter(list[i]));
        }

        return message.ToString();
    }

    /// <summary>
    /// Returns the hex value of the color.
    /// </summary>
    /// <param name="color"></param>
    /// <returns>The value of the color as a hex string.</returns>
    public static string ToHex(this Color color)
    {
        return ((byte)(color.r * 255)).ToString("X2") +
                ((byte)(color.g * 255)).ToString("X2") +
                ((byte)(color.b * 255)).ToString("X2") +
                ((byte)(color.a * 255)).ToString("X2");
    }
}
