using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskUtils
{
    public static string Print(this LayerMask layerMask)
    {
        string message = string.Empty;

        for (int i = 0; i < 32; i++)
        {
            if ((layerMask.value & (1 << i)) != 0)
            {
                string name = LayerMask.LayerToName(i);

                if (!string.IsNullOrEmpty(name))
                {
                    message += $"{name}, ";
                }
            }
        }

        return message;
    }

    public static LayerMask AddLayer(this LayerMask mask, int layer)
    {
        return mask | (1 << layer);
    }

    public static bool HasLayerMask(this int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    public static bool HasLayerMask(this GameObject gameObject, LayerMask mask)
    {
        return (mask.value & (1 << gameObject.layer)) != 0;
    }

    /// <summary>
    /// Gets the world position of the mouse cursor.
    /// </summary>
    /// <param name="mouseColliderLayerMask">The layer mask to use for the raycast.</param>
    /// <returns>The world position of the mouse cursor.</returns>
    public static Vector3 GetMouseWorldPosition(LayerMask mouseColliderLayerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 999f, mouseColliderLayerMask))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
