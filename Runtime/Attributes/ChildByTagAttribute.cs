#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CustomAttributes
{
    public class ChildByTagAttribute : PropertyAttribute
    {
        public string Tag { get; }

        public ChildByTagAttribute(string tag)
        {
            Tag = tag;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ChildByTagAttribute))]
    public class ChildByTagPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var tagAttr = (ChildByTagAttribute)attribute;
            var component = property.serializedObject.targetObject as Component;

            if (component != null && property.propertyType == SerializedPropertyType.ObjectReference)
            {
                Transform found = FindChildWithTag(component.transform, tagAttr.Tag);
                if (found != null)
                {
                    property.objectReferenceValue = found;
                }
            }

            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndProperty();
        }

        private Transform FindChildWithTag(Transform parent, string tag)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag(tag))
                {
                    return child;
                }
            }

            return null;
        }
    }
#endif
}