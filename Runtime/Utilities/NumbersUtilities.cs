using UnityEngine;
using Random = UnityEngine.Random;

public static class NumbersUtilities
{
    /// <summary>
    /// Checks whether the value is within the specified range. The check is inclusive on both ends.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The inclusive minimum of the range.</param>
    /// <param name="max">The inclusive maximum of the range.</param>
    /// <returns>True if <paramref name="value"/> is between <paramref name="min"/> and <paramref name="max"/> (inclusive).</returns>
    public static bool IsInRange(this int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Returns a random float in the symmetric range around zero defined by the given value, i.e. (-value, value).
    /// </summary>
    /// <param name="value">The magnitude defining the symmetric range.</param>
    /// <returns>A random float between -<paramref name="value"/> and <paramref name="value"/>.</returns>
    public static float GetRandomValue(this float value)
    {
        return Random.Range(-value, value);
    }

    /// <summary>
    /// Treats the value as a probability in the [0, 1] range and returns true if a random roll succeeds.
    /// </summary>
    /// <param name="value">The probability of success, where 0 is never and 1 is always.</param>
    /// <returns>True if a random roll in [0, 1) is less than <paramref name="value"/>.</returns>
    public static bool IsChanceSuccessful(this float value)
    {
        return Random.Range(0f, 1f) < value;
    }

    /// <summary>
    /// Adds <paramref name="valueToAdd"/> to <paramref name="value"/> and wraps the result around the specified inclusive range.
    /// If the sum goes below <paramref name="minRange"/>, it wraps to <paramref name="maxRange"/>, and vice versa.
    /// </summary>
    /// <param name="value">The initial value.</param>
    /// <param name="valueToAdd">The value to add (can be negative).</param>
    /// <param name="minRange">The inclusive minimum of the wrap range.</param>
    /// <param name="maxRange">The inclusive maximum of the wrap range.</param>
    /// <returns>The resulting value within the specified range, wrapping at the bounds.</returns>
    public static int AddInRange(int value, int valueToAdd, int minRange, int maxRange)
    {
        value += valueToAdd;

        if (value < minRange)
        {
            value = maxRange;
        }
        else if (value > maxRange)
        {
            value = minRange;
        }
        else
        {
            value = Mathf.Clamp(value, minRange, maxRange);
        }

        return value;
    }

    /// <summary>
    /// Converts a duration in seconds to whole minutes, truncating any remainder.
    /// </summary>
    /// <param name="currentTime">The duration in seconds.</param>
    /// <returns>The number of whole minutes contained in <paramref name="currentTime"/>.</returns>
    public static int SecondsToMinutes(float currentTime)
    {
        return Mathf.FloorToInt(currentTime / 60f);
    }

    /// <summary>
    /// Converts a duration in seconds to whole hours, truncating any remainder.
    /// </summary>
    /// <param name="currentTime">The duration in seconds.</param>
    /// <returns>The number of whole hours contained in <paramref name="currentTime"/>.</returns>
    public static int SecondsToHours(float currentTime)
    {
        return Mathf.FloorToInt(currentTime / 60f / 60f);
    }
    
    public static void Log(this int value)
    {
        Debug.Log(value);
    }

    public static void Log(this float value)
    {
        Debug.Log(value);
    }
}