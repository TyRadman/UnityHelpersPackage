#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

namespace TankLike
{
    public class HasInterface : PropertyAttribute
    {
        public Type InterfaceType { get; }

        public HasInterface(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("Provided type is not an interface", nameof(interfaceType));
            }

            InterfaceType = interfaceType;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HasInterface))]
    public class HasInterfacePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HasInterface checkInterface = (HasInterface)attribute;

            // Draw the property field
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label);

            // Validate if the assigned object implements the specified interface
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                MonoBehaviour assignedObject = property.objectReferenceValue as MonoBehaviour;
                if (assignedObject != null)
                {
                    if (!checkInterface.InterfaceType.IsAssignableFrom(assignedObject.GetType()))
                    {
                        // Draw red overlay to indicate an error
                        EditorGUI.DrawRect(position, new Color(1f, 0.5f, 0.5f, 0.5f));

                        // Tooltip on hover
                        GUIContent errorContent = new GUIContent("", $"The assigned object does not implement the {checkInterface.InterfaceType.Name} interface.");
                        EditorGUI.LabelField(position, errorContent);
                    }
                }
            }
            else if (property.propertyType != SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            {
                Debug.LogWarning($"CheckInterface can only be applied to ObjectReference fields like MonoBehaviour.");
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}
