using System.Collections;
using System.Collections.Generic;
using CustomAttributes;
using UnityEditor;
using UnityEngine;

namespace TankLike
{
    [CustomPropertyDrawer(typeof(EmissiveAttribute))]
    public class EmissivePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            property.colorValue = EditorGUI.ColorField(position, label, property.colorValue, true, true, true);
            EditorGUI.EndProperty();
        }
    }
}
