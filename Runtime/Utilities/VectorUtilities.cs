using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public static class VectorUtilities
{
   /// <summary>
    /// Returns a random float value between the vector's x and y values.
    /// </summary>
    /// <param name="_value">The Vector2 containing the range (x, y).</param>
    /// <returns>A random float value between x and y.</returns>
    public static float RandomValue(this Vector2 _value)
    {
        return Random.Range(_value.x, _value.y);
    }

    /// <summary>
    /// Returns a random integer value between the vector's x and y values.
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_float"></param>
    /// <returns></returns>
    public static bool HasFloatInRange(this Vector2 _value, float _float)
    {
        return _float >= _value.x && _float <= _value.y;
    }

    /// <summary>
    /// Linearly interpolates between the x and y values of the vector based on the given t value.
    /// </summary>
    /// <param name="_value">The Vector2 containing the range (x, y).</param>
    /// <param name="_tValue">The interpolation factor (0 to 1).</param>
    /// <returns>The interpolated float value.</returns>
    public static float Lerp(this Vector2 _value, float _tValue)
    {
        return Mathf.Lerp(_value.x, _value.y, _tValue);
    }
    
    /// <summary>
    /// Linearly interpolates between the x and y values of the vector and returns the result as an integer.
    /// </summary>
    /// <param name="_value">The Vector2Int containing the range (x, y).</param>
    /// <param name="_tValue">The interpolation factor (0 to 1).</param>
    /// <returns>The interpolated integer value.</returns>
    public static int Lerp(this Vector2Int _value, float _tValue)
    {
        return (int)Mathf.Lerp(_value.x, _value.y, _tValue);
    }
    
    /// <summary>
    /// Returns a random integer value between the vector's x and y values.
    /// </summary>
    /// <param name="_value">The Vector2Int containing the range (x, y).</param>
    /// <returns>A random integer value between x and y (inclusive).</returns>
    public static int RandomValue(this Vector2Int _value)
    {
        return Random.Range(_value.x, _value.y + 1);
    }
    
    /// <summary>
    /// Returns a new vector with the given values.
    /// </summary>
    /// <param name="vector">The original vector.</param>
    /// <param name="x">The new x value (optional).</param>
    /// <param name="y">The new y value (optional).</param>
    /// <param name="z">The new z value (optional).</param>
    /// <returns>A new vector with the specified values.</returns>
    public static Vector3 Where(this Vector3 vector, float x = float.NaN, float y = float.NaN, float z = float.NaN)
    {
        return new Vector3(
            float.IsNaN(x) ? vector.x : x,
            float.IsNaN(y) ? vector.y : y,
            float.IsNaN(z) ? vector.z : z);
    }

    /// <summary>
    /// Adds given values to the vector.
    /// </summary>
    /// <param name="vector">The original vector.</param>
    /// <param name="x">The value to add to the x component (optional).</param>
    /// <param name="y">The value to add to the y component (optional).</param>
    /// <param name="z">The value to add to the z component (optional).</param>
    /// <returns>A new vector with the added values.</returns>
    public static Vector3 Add(this Vector3 vector, float x = float.NaN, float y = float.NaN, float z = float.NaN)
    {
        return new Vector3(
            vector.x + (float.IsNaN(x) ? 0 : x),
            vector.y + (float.IsNaN(y) ? 0 : y),
            vector.z + (float.IsNaN(z) ? 0 : z));
    }
}
