using UnityEngine;

namespace CustomAttributes
{
    /// <summary>
    /// A property that displays the variable's in-code value without allowing it to be changed through the inspector
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }
}
