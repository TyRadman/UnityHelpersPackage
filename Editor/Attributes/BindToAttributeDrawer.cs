using UnityEditor;
using System.Reflection;
using UnityEngine;
using CustomAttributes;

namespace CustomAttributes.Editor
{
    [CustomPropertyDrawer(typeof(BindToAttribute))]
    public class BindToPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);

            if (EditorGUI.EndChangeCheck())
            {
                Object target = property.serializedObject.targetObject;
                string methodName = ((BindToAttribute)attribute).MethodName;

                MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (method != null)
                {
                    method.Invoke(target, null);
                }
                else
                {
                    Debug.Log($"No method with the name {methodName} exists");
                }
            }
        }
    }
}
