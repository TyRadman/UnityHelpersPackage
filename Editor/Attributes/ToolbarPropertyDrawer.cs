using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ToolbarAttribute))]
public class ToolbarPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Check if the property this attribute is attached to is actually an Enum
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            // Draw the prefix label (the name of the variable) and get the remaining space
            Rect controlRect = EditorGUI.PrefixLabel(position, label);

            // Draw the toolbar using the enum's display names
            EditorGUI.BeginChangeCheck();
            
            int newIndex = GUI.Toolbar(controlRect, property.enumValueIndex, property.enumDisplayNames);
            
            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = newIndex;
            }
        }
        else
        {
            // If it's not an enum, just draw the default property field (do nothing special)
            EditorGUI.PropertyField(position, property, label);
        }
    }
}