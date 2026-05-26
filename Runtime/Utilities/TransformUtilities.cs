using UnityEngine;

/// <summary>
/// Extension methods for working with <see cref="Transform"/>.
/// </summary>
public static class TransformUtilities
{
    /// <summary>
    /// Moves the parent Transform to the child's world position without affecting the child's world position.
    /// </summary>
    /// <param name="child">The child Transform whose parent will be repositioned.</param>
    public static void ResetParentPositionToChild(this Transform child)
    {
        if (child.parent == null)
        {
            return;
        }

        Vector3 childWorldPos = child.position;
        Transform parent = child.parent;

        child.SetParent(null, true);
        parent.position = childWorldPos;
        child.SetParent(parent, true);
    }

    /// <summary>
    /// Checks whether <paramref name="source"/> is facing <paramref name="point"/> within the given field-of-view angle.
    /// </summary>
    /// <param name="source">The Transform whose forward direction is tested.</param>
    /// <param name="point">The target world-space point.</param>
    /// <param name="angle">The full field-of-view angle in degrees (e.g. 90 means ±45° from forward).</param>
    /// <returns>True if <paramref name="point"/> lies within the cone defined by <paramref name="angle"/> around <paramref name="source"/>'s forward axis.</returns>
    public static bool IsFacingPoint(this Transform source, Vector3 point, float angle)
    {
        Vector3 direction = (point - source.position).normalized;
        float dot = Vector3.Dot(source.forward, direction);
        return dot >= Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Returns the distance from this Transform's position to the given world-space point.
    /// </summary>
    /// <param name="transform">The Transform to measure from.</param>
    /// <param name="position">The world-space point to measure to.</param>
    /// <returns>The straight-line distance between the two points.</returns>
    public static float GetDistanceTo(this Transform transform, Vector3 position)
    {
        return Vector3.Distance(transform.position, position);
    }
}