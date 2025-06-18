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
}