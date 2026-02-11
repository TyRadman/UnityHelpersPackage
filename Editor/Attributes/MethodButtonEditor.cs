using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public sealed class MethodButtonEditor : Editor
{
    private static readonly Dictionary<string, object> _paramCache = new();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Type targetType = base.target.GetType();
        var methods = GetButtonMethods(targetType);

        if (methods.Count == 0)
        {
            return;
        }

        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        foreach (var (method, attribute) in methods)
        {
            DrawMethodBlock(method, attribute, targetType);
        }
    }

    private static List<(MethodInfo method, MethodButtonAttribute attribute)> GetButtonMethods(Type type)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        return type.GetMethods(flags)
            .Select(m => (method: m, attribute: m.GetCustomAttribute<MethodButtonAttribute>(true)))
            .Where(x =>
                x.attribute != null &&
                !x.method.IsAbstract &&
                !x.method.IsGenericMethodDefinition &&
                !x.method.IsSpecialName &&
                !x.method.IsStatic)
            .ToList();
    }

    private void DrawMethodBlock(MethodInfo method, MethodButtonAttribute attribute, Type targetType)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.BeginVertical();

        string buttonLabel = string.IsNullOrWhiteSpace(attribute.Label) ? ObjectNames.NicifyVariableName(method.Name) : attribute.Label;

        bool canInvoke = CanInvoke(method, out string cantInvokeReason);

        using (new EditorGUI.DisabledScope(!canInvoke))
        {
            if (GUILayout.Button(buttonLabel))
            {
                InvokeForTargets(method, targetType);
            }
        }

        GUIStyle style = new GUIStyle(EditorStyles.miniLabel);
        style.richText = true;
        EditorGUILayout.LabelField($"<color=#32a8a6>{GetAccessModifier(method)}</color> <color=#a83275>{method.ReturnType.Name}</color> <b>{method.Name}</b> ()", style);

        var parameters = method.GetParameters();
        if (parameters.Length > 0)
        {
            EditorGUILayout.Space(4f);

            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];

                using (new EditorGUI.DisabledScope(p.IsOut || p.ParameterType.IsByRef))
                {
                    DrawParameterField(targetType, method, p, i);
                }
            }
        }

        if (!canInvoke && !string.IsNullOrEmpty(cantInvokeReason))
        {
            EditorGUILayout.Space(4f);
            EditorGUILayout.HelpBox(cantInvokeReason, MessageType.Warning);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private static bool CanInvoke(MethodInfo method, out string reason)
    {
        reason = null;

        ParameterInfo[] parameters = method.GetParameters();

        for (int i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];

            if (p.IsOut || p.ParameterType.IsByRef)
            {
                reason = "ref/out parameters are not supported.";
                return false;
            }

            if (!IsSupportedParamType(p.ParameterType))
            {
                reason = $"Unsupported parameter type: {p.ParameterType.Name}";
                return false;
            }
        }

        return true;
    }

    private void InvokeForTargets(MethodInfo method, Type targetType)
    {
        ParameterInfo[] parameters = method.GetParameters();
        object[] args = parameters.Length == 0 ? null : new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            string key = MakeParamKey(targetType, method, i);
            args[i] = GetCachedOrDefault(key, parameters[i].ParameterType);
        }

        foreach (UnityEngine.Object obj in targets)
        {
            try
            {
                Undo.RecordObject(obj, method.Name);
                method.Invoke(obj, args);
                EditorUtility.SetDirty(obj);
            }
            catch (TargetInvocationException tie)
            {
                Debug.LogException(tie.InnerException ?? tie, obj);
            }
            catch (Exception e)
            {
                Debug.LogException(e, obj);
            }
        }
    }

    private static void DrawParameterField(Type targetType, MethodInfo method, ParameterInfo p, int paramIndex)
    {
        string key = MakeParamKey(targetType, method, paramIndex);
        Type paramType = p.ParameterType;

        object value = GetCachedOrDefault(key, paramType);
        object newValue = DrawFieldForType(ObjectNames.NicifyVariableName(p.Name), paramType, value);

        if (!EqualsBoxed(value, newValue))
            _paramCache[key] = newValue;
    }

    private static object DrawFieldForType(string label, Type type, object value)
    {
        if (type == typeof(int)) return EditorGUILayout.IntField(label, SafeCast<int>(value));
        if (type == typeof(float)) return EditorGUILayout.FloatField(label, SafeCast<float>(value));
        if (type == typeof(double)) return EditorGUILayout.DoubleField(label, SafeCast<double>(value));
        if (type == typeof(bool)) return EditorGUILayout.Toggle(label, SafeCast<bool>(value));
        if (type == typeof(string)) return EditorGUILayout.TextField(label, value as string ?? string.Empty);

        if (type == typeof(Vector2)) return EditorGUILayout.Vector2Field(label, SafeCast<Vector2>(value));
        if (type == typeof(Vector3)) return EditorGUILayout.Vector3Field(label, SafeCast<Vector3>(value));
        if (type == typeof(Vector4)) return EditorGUILayout.Vector4Field(label, SafeCast<Vector4>(value));
        if (type == typeof(Color)) return EditorGUILayout.ColorField(label, SafeCast<Color>(value));
        if (type == typeof(Rect)) return EditorGUILayout.RectField(label, SafeCast<Rect>(value));
        if (type == typeof(Bounds)) return EditorGUILayout.BoundsField(label, SafeCast<Bounds>(value));
        if (type == typeof(AnimationCurve)) return EditorGUILayout.CurveField(label, value as AnimationCurve ?? new AnimationCurve());

        if (type == typeof(LayerMask))
        {
            var lm = SafeCast<LayerMask>(value);
            int mask = EditorGUILayout.MaskField(label, lm.value, InternalEditorUtility.layers);
            lm.value = mask;
            return lm;
        }

        if (type.IsEnum)
        {
            if (value == null) value = Activator.CreateInstance(type);
            return EditorGUILayout.EnumPopup(label, (Enum)value);
        }

        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            var obj = value as UnityEngine.Object;
            return EditorGUILayout.ObjectField(label, obj, type, true);
        }

        EditorGUILayout.LabelField(label, $"Unsupported: {type.Name}");
        return value;
    }

    private static bool IsSupportedParamType(Type type)
    {
        if (type == typeof(int) ||
            type == typeof(float) ||
            type == typeof(double) ||
            type == typeof(bool) ||
            type == typeof(string) ||
            type == typeof(Vector2) ||
            type == typeof(Vector3) ||
            type == typeof(Vector4) ||
            type == typeof(Color) ||
            type == typeof(Rect) ||
            type == typeof(Bounds) ||
            type == typeof(AnimationCurve) ||
            type == typeof(LayerMask))
            return true;

        if (type.IsEnum) return true;
        if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return true;

        return false;
    }

    private static string GetAccessModifier(MethodInfo m)
    {
        if (m.IsPublic) return "public";
        if (m.IsFamily) return "protected";
        if (m.IsFamilyOrAssembly) return "protected internal";
        if (m.IsAssembly) return "internal";
        if (m.IsPrivate) return "private";
        return "private";
    }

    private static string MakeParamKey(Type targetType, MethodInfo method, int paramIndex)
    {
        // stable enough across domain reloads: type + method name + param types + index
        string sig = method.Name + "(" + string.Join(",", method.GetParameters().Select(p => p.ParameterType.FullName)) + ")";
        return $"{targetType.AssemblyQualifiedName}|{sig}|{paramIndex}";
    }

    private static object GetCachedOrDefault(string key, Type type)
    {
        if (_paramCache.TryGetValue(key, out var v) && v != null && type.IsInstanceOfType(v))
        {
            return v;
        }

        object def = CreateDefault(type);
        _paramCache[key] = def;
        return def;
    }

    private static object CreateDefault(Type type)
    {
        if (type == typeof(string)) return string.Empty;
        if (type == typeof(AnimationCurve)) return new AnimationCurve();

        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            return null;

        if (type.IsEnum)
            return Activator.CreateInstance(type);

        if (type == typeof(LayerMask))
            return default(LayerMask);

        if (type.IsValueType)
            return Activator.CreateInstance(type);

        return null;
    }

    private static T SafeCast<T>(object o)
    {
        if (o is T t) return t;
        return default;
    }

    private static bool EqualsBoxed(object a, object b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null || b == null) return false;
        return a.Equals(b);
    }
}
