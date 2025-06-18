#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CustomAttributes
{
    public class EmissiveAttribute : PropertyAttribute { }

#if UNITY_EDITOR
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
#endif
}
