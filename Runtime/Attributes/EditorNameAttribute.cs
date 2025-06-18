using UnityEngine;

namespace CustomAttributes
{
    public class EditorNameAttribute : PropertyAttribute
    {
        public string DisplayName { get; }

        public EditorNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}