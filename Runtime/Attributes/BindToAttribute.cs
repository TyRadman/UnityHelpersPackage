#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using UnityEngine;

namespace CustomAttributes
{
    public class BindToAttribute : PropertyAttribute
    {
        public string MethodName;

        public BindToAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }

#if UNITY_EDITOR
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
#endif
}