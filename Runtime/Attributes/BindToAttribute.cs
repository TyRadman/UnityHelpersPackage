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
}