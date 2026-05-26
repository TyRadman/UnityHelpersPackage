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

    /// <summary>
    /// Converts a <see cref="Vector2"/> to a <see cref="Vector2Int"/> by truncating the float components.
    /// </summary>
    /// <param name="value">The Vector2 to convert.</param>
    /// <returns>A Vector2Int with the truncated x and y values.</returns>
    public static Vector2Int ToVector2Int(this Vector2 value)
    {
        return new Vector2Int((int)value.x, (int)value.y);
    }

    /// <summary>
    /// Converts a <see cref="Vector2Int"/> to a <see cref="Vector3"/>, mapping x to x and y to z (XZ plane), with y set to 0.
    /// </summary>
    /// <param name="value">The Vector2Int to convert.</param>
    /// <returns>A Vector3 on the XZ plane.</returns>
    public static Vector3 ToVector3(this Vector2Int value)
    {
        return new Vector3(value.x, 0f, value.y);
    }

    /// <summary>
    /// Converts a <see cref="Vector3"/> to a <see cref="Vector2"/> by dropping the z component.
    /// </summary>
    /// <param name="vector">The Vector3 to convert.</param>
    /// <returns>A Vector2 with the x and y values of the original vector.</returns>
    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    /// <summary>
    /// Returns a random point inside a sphere with the given center and radius.
    /// </summary>
    /// <param name="center">The center of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <returns>A random point inside the sphere.</returns>
    public static Vector3 GetRandomPointInsideSphere(Vector3 center, float radius)
    {
        return center + Random.insideUnitSphere * radius;
    }

    /// <summary>
    /// Returns the centroid (average position) of any number of points.
    /// </summary>
    /// <param name="points">The list of points to average.</param>
    /// <returns>The centroid of the provided points, or <see cref="Vector3.zero"/> if the list is null or empty.</returns>
    public static Vector3 GetCentroid(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
        {
            Debug.LogWarning("GetCentroid called with empty or null point list.");
            return Vector3.zero;
        }

        Vector3 sum = Vector3.zero;
        for (int i = 0; i < points.Count; i++)
        {
            sum += points[i];
        }

        return sum / points.Count;
    }
}
