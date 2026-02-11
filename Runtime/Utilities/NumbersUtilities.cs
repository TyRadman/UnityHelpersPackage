using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class NumbersUtilities
{
    /// <summary>
    /// Returns a random integer value between the vector's x and y values. The check is inclusive.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static bool IsInRange(this int value, int min, int max)
    {
        return value >= min && value <= max;
    }
}
