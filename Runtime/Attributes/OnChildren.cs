#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CustomAttributes
{
    /// <summary>
    /// Attribute to get a reference to a component in the children of the GameObject.
    /// </summary>
    public class OnChildren : PropertyAttribute
    {
        /// <summary>
        /// If true, the attribute will add a child gameobject with the required component if it doesn't exist.
        /// </summary>
        public bool ForceReference { get; }
        public int InstanceIndex { get; }

        public OnChildren(bool forceReference = false, int instanceIndex = 0)
        {
            ForceReference = forceReference;
            InstanceIndex = instanceIndex;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(OnChildren))]
    public class InChildrenDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                GameObject parent = (property.serializedObject.targetObject as MonoBehaviour)?.gameObject;

                if (parent != null)
                {
                    OnChildren att = (attribute as OnChildren);
                    System.Type fieldType = fieldInfo.FieldType;
                    Component[] components = parent.GetComponentsInChildren(fieldType, includeInactive: true);

                    if (components.Length == 0)
                    {
                        return;
                    }

                    Component component = components[Mathf.Min(components.Length - 1, att.InstanceIndex)];

                    if (component != null)
                    {
                        property.objectReferenceValue = component;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    else if (att.ForceReference)
                    {
                        GameObject newChild = new GameObject(fieldType.Name);
                        newChild.transform.parent = parent.transform;
                        Component newComponent = newChild.AddComponent(fieldType);
                        property.objectReferenceValue = newComponent;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
#endif
}
