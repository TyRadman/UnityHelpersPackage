#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CustomAttributes
{
    /// <summary>
    /// Attribute to get a reference to a component in the GameObject itself.
    /// </summary>
    public class OnSelf : PropertyAttribute
    {
        /// <summary>
        /// If true, the attribute will add the component to the GameObject if it doesn't exist.
        /// </summary>
        public bool ForceReference { get; }

        public OnSelf(bool forceReference = false)
        {
            ForceReference = forceReference;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(OnSelf))]
    public class SelfReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                GameObject targetGameObject = (property.serializedObject.targetObject as MonoBehaviour)?.gameObject;

                if (targetGameObject != null)
                {
                    System.Type fieldType = fieldInfo.FieldType;
                    Component component = targetGameObject.GetComponent(fieldType);

                    if (component == null)
                    {
                        OnSelf inSelf = attribute as OnSelf;

                        if (inSelf.ForceReference)
                        {
                            component = targetGameObject.AddComponent(fieldType);
                        }
                    }

                    if (component != null)
                    {
                        property.objectReferenceValue = component;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
#endif
}
