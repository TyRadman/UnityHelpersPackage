// Place this file in an "Editor" folder.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DynamicEnumAttribute))]
public class DynamicEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Enum)
        {
            EditorGUI.LabelField(position, label.text, "[DynamicEnum] only works on enums.");
            return;
        }

        Type enumType = GetEnumType();
        if (enumType == null)
        {
            EditorGUI.LabelField(position, label.text, "Could not resolve enum type.");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);
        Rect fieldRect = EditorGUI.PrefixLabel(position, label);

        string[] names = Enum.GetNames(enumType);
        int currentIndex = property.enumValueIndex;
        string current = (currentIndex >= 0 && currentIndex < names.Length)
            ? names[currentIndex]
            : "<invalid>";

        if (EditorGUI.DropdownButton(fieldRect, new GUIContent(current), FocusType.Keyboard))
        {
            PopupWindow.Show(fieldRect, new DynamicEnumPopup(enumType, property, fieldRect.width));
        }

        EditorGUI.EndProperty();
    }

    private Type GetEnumType()
    {
        Type t = fieldInfo.FieldType;
        if (t.IsArray) 
        {
            t = t.GetElementType();
        }
        else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
        {
            t = t.GetGenericArguments()[0];
        }
            
        return (t != null && t.IsEnum) ? t : null;
    }

    private class DynamicEnumPopup : PopupWindowContent
    {
        private const float RowHeight = 20f;
        private const int CheckColumnWidth = 18;

        private readonly Type _enumType;
        private readonly SerializedProperty _property;
        private readonly float _width;

        private readonly string[] _names;
        private readonly int[] _values;

        private Vector2 _scroll;

        // Add-value sub-form state
        private bool _addMode;
        private string _newName = "";
        private string _newValueText = "";

        public DynamicEnumPopup(Type enumType, SerializedProperty property, float width)
        {
            _enumType = enumType;
            _property = property.Copy();
            _width = Mathf.Max(width, 230f);
            _names = Enum.GetNames(enumType);
            _values = Enum.GetValues(enumType).Cast<object>()
                          .Select(v => Convert.ToInt32(v)).ToArray();
        }

        public override Vector2 GetWindowSize()
        {
            float listHeight = (_names.Length + 1) * RowHeight + 20f;
            if (_addMode) listHeight += 110f;
            return new Vector2(_width, Mathf.Min(listHeight, 420f));
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(4, 4, 6, 6) });
            _scroll = GUILayout.BeginScrollView(_scroll);

            var orange = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(CheckColumnWidth + 4, 4, 0, 0)
            };
            orange.normal.textColor = new Color(1f, 0.6f, 0.1f);
            orange.hover.textColor = new Color(1f, 0.75f, 0.3f);

            if (GUILayout.Button("Add value", orange, GUILayout.Height(RowHeight)))
            {
                _addMode = !_addMode;
                _newName = "";
                _newValueText = "";
                GUI.FocusControl(null);
            }

            if (_addMode)
                DrawAddForm();

            for (int i = 0; i < _names.Length; i++)
            {
                GUILayout.BeginHorizontal();

                bool selected = _property.enumValueIndex == i;

                // Fixed-width check column so text never shifts.
                var checkStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                GUILayout.Label(selected ? "\u2713" : "", checkStyle,
                    GUILayout.Width(CheckColumnWidth), GUILayout.Height(RowHeight));

                var rowStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(0, 4, 0, 0)
                };
                if (selected) rowStyle.fontStyle = FontStyle.Bold;

                if (GUILayout.Button($"{_names[i]} ({_values[i]})",
                        rowStyle, GUILayout.Height(RowHeight)))
                {
                    _property.serializedObject.Update();
                    _property.enumValueIndex = i;
                    _property.serializedObject.ApplyModifiedProperties();
                    editorWindow.Close();
                }

                var xStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
                xStyle.normal.textColor = new Color(1f, 0.35f, 0.35f);
                xStyle.hover.textColor = new Color(1f, 0.55f, 0.55f);

                if (GUILayout.Button("\u00D7", xStyle, GUILayout.Width(18f), GUILayout.Height(RowHeight)))
                {
                    string name = _names[i];
                    if (EditorUtility.DisplayDialog(
                            "Delete enum value",
                            $"Delete '{name}' from enum '{_enumType.Name}'?\n\n" +
                            "This will modify the source file and recompile. " +
                            "Fields currently using this value may change meaning.",
                            "Delete", "Cancel"))
                    {
                        editorWindow.Close();
                        DynamicEnumSourceEditor.RemoveValue(_enumType, name);
                    }
                    GUIUtility.ExitGUI();
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawAddForm()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            string error = Validate(out int? parsedValue);
            bool invalid = error != null && (_newName.Length > 0 || _newValueText.Length > 0);

            var nameStyle = new GUIStyle(EditorStyles.textField);
            var valueStyle = new GUIStyle(EditorStyles.textField);
            if (invalid)
            {
                // highlight offending boxes in red
                if (error.Contains("name")) nameStyle.normal.textColor = nameStyle.focused.textColor = Color.red;
                if (error.Contains("value")) valueStyle.normal.textColor = valueStyle.focused.textColor = Color.red;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.Width(90f));
            _newName = GUILayout.TextField(_newName, nameStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Value (Optional)", GUILayout.Width(90f));
            _newValueText = GUILayout.TextField(_newValueText, valueStyle);
            GUILayout.EndHorizontal();

            if (invalid)
            {
                var errStyle = new GUIStyle(EditorStyles.miniLabel);
                errStyle.normal.textColor = Color.red;
                GUILayout.Label(error, errStyle);
            }

            GUILayout.BeginHorizontal();
            GUI.enabled = error == null && !string.IsNullOrWhiteSpace(_newName);
            if (GUILayout.Button("Save"))
            {
                GUI.enabled = true;
                editorWindow.Close();
                DynamicEnumSourceEditor.AddValue(_enumType, _newName.Trim(), parsedValue);
                GUIUtility.ExitGUI();
            }
            GUI.enabled = true;
            if (GUILayout.Button("Cancel"))
            {
                _addMode = false;
                _newName = "";
                _newValueText = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private string Validate(out int? parsedValue)
        {
            parsedValue = null;
            string trimmed = _newName.Trim();

            if (string.IsNullOrEmpty(trimmed))
                return "A name is required.";
            if (!Regex.IsMatch(trimmed, @"^[A-Za-z_][A-Za-z0-9_]*$"))
                return "Invalid name (must be a valid C# identifier).";
            if (_names.Any(n => string.Equals(n, trimmed, StringComparison.Ordinal)))
                return "That name already exists.";

            if (!string.IsNullOrWhiteSpace(_newValueText))
            {
                if (!int.TryParse(_newValueText.Trim(), out int v))
                    return "Invalid value (must be an integer).";
                if (v < 0)
                    return "Negative values are not allowed.";
                if (_values.Contains(v))
                    return "That value already exists.";
                parsedValue = v;
            }
            return null;
        }
    }

    private static class DynamicEnumSourceEditor
    {
        public static void AddValue(Type enumType, string name, int? explicitValue)
        {
            if (!TryLocateEnum(enumType, out string path, out string source,
                               out int bodyStart, out int bodyEnd))
                return;

            string body = source.Substring(bodyStart, bodyEnd - bodyStart);

            // Figure out indentation from an existing member line, fallback to 8 spaces.
            string indent = "        ";
            var indentMatch = Regex.Match(body, @"(?m)^([ \t]+)[A-Za-z_]");
            if (indentMatch.Success) indent = indentMatch.Groups[1].Value;

            int[] existing = Enum.GetValues(enumType).Cast<object>()
                                 .Select(v => Convert.ToInt32(v)).ToArray();
            string valuePart;
            if (explicitValue.HasValue)
            {
                valuePart = " = " + explicitValue.Value;
            }
            else
            {
                bool anyExplicit = Regex.IsMatch(body, @"=\s*-?\d");
                valuePart = anyExplicit
                    ? " = " + (existing.Length > 0 ? existing.Max() + 1 : 0)
                    : "";
            }

            string trimmedBody = body.TrimEnd();
            string newBody;
            bool bodyHasMembers = Regex.IsMatch(trimmedBody, @"[A-Za-z_][A-Za-z0-9_]*");
            if (bodyHasMembers && !trimmedBody.EndsWith(","))
            {
                // append comma after last non-comment token
                int lastIdx = LastMemberEnd(trimmedBody);
                trimmedBody = trimmedBody.Insert(lastIdx, ",");
            }
            string closingIndent = indent.Length >= 4 ? indent.Substring(0, indent.Length - 4) : "";
            newBody = trimmedBody + Environment.NewLine + indent + name + valuePart
                      + Environment.NewLine + closingIndent;

            WriteAndRefresh(path, source.Substring(0, bodyStart) + newBody + source.Substring(bodyEnd));
        }

        public static void RemoveValue(Type enumType, string name)
        {
            if (!TryLocateEnum(enumType, out string path, out string source,
                               out int bodyStart, out int bodyEnd))
                return;

            string body = source.Substring(bodyStart, bodyEnd - bodyStart);

            string pattern =
                @"(?m)^[ \t]*(?:\[[^\]]*\][ \t]*\r?\n[ \t]*)*" +    // attributes on their own lines
                @"(?:\[[^\]]*\][ \t]*)*" +                          // inline attributes
                Regex.Escape(name) +
                @"\b[ \t]*(?:=[^,}\r\n]*)?" +                       // optional "= value"
                @"[ \t]*,?[ \t]*(?://[^\r\n]*)?(?:\r?\n)?";          // comma + trailing comment + newline

            var match = Regex.Match(body, pattern);
            if (!match.Success)
            {
                pattern = @"\b" + Regex.Escape(name) + @"\b[ \t]*(?:=[^,}\r\n]*)?[ \t]*,?[ \t]*";
                match = Regex.Match(body, pattern);
                if (!match.Success)
                {
                    EditorUtility.DisplayDialog("DynamicEnum",
                        $"Could not find '{name}' inside enum '{enumType.Name}' in:\n{path}", "OK");
                    return;
                }
            }

            string newBody = body.Remove(match.Index, match.Length);

            // Clean up a dangling comma before the closing brace, e.g. "A, B, }" -> "A, B }"
            newBody = Regex.Replace(newBody, @",(\s*)$", "$1");

            WriteAndRefresh(path, source.Substring(0, bodyStart) + newBody + source.Substring(bodyEnd));
        }

        private static void WriteAndRefresh(string path, string newSource)
        {
            File.WriteAllText(path, newSource);
            AssetDatabase.ImportAsset(ToAssetPath(path));
            AssetDatabase.Refresh();
        }

        private static bool TryLocateEnum(Type enumType, out string path,
                                          out string source, out int bodyStart, out int bodyEnd)
        {
            path = null; source = null; bodyStart = bodyEnd = -1;

            string enumName = enumType.Name;
            // "enum" keyword, the exact name, optional ": underlyingType", then "{".
            var declRegex = new Regex(
                @"\benum\s+" + Regex.Escape(enumName) +
                @"\b\s*(?::\s*[A-Za-z_][A-Za-z0-9_.]*\s*)?\{",
                RegexOptions.Singleline);

            foreach (string file in Directory.EnumerateFiles(Application.dataPath, "*.cs",
                         SearchOption.AllDirectories))
            {
                string text;
                try { text = File.ReadAllText(file); }
                catch { continue; }

                if (!text.Contains(enumName)) continue;

                foreach (Match m in declRegex.Matches(text))
                {
                    int openBrace = m.Index + m.Length - 1;

                    if (enumType.DeclaringType != null &&
                        text.LastIndexOf(enumType.DeclaringType.Name, m.Index, StringComparison.Ordinal) < 0)
                        continue;
                    if (!string.IsNullOrEmpty(enumType.Namespace) &&
                        text.Contains("namespace ") &&
                        text.LastIndexOf(enumType.Namespace.Split('.').Last(), m.Index,
                            StringComparison.Ordinal) < 0)
                        continue;

                    int close = FindMatchingBrace(text, openBrace);
                    if (close < 0) continue;

                    path = file;
                    source = text;
                    bodyStart = openBrace + 1;
                    bodyEnd = close;
                    return true;
                }
            }

            EditorUtility.DisplayDialog("DynamicEnum",
                $"Could not locate the source file declaring enum '{enumType.FullName}'.\n" +
                "Note: enums in packages or precompiled assemblies cannot be edited.", "OK");
            return false;
        }

        private static int FindMatchingBrace(string text, int openIndex)
        {
            int depth = 0;
            bool inString = false, inChar = false, inLineComment = false, inBlockComment = false;
            for (int i = openIndex; i < text.Length; i++)
            {
                char c = text[i];
                char next = i + 1 < text.Length ? text[i + 1] : '\0';

                if (inLineComment) { if (c == '\n') inLineComment = false; continue; }
                if (inBlockComment) { if (c == '*' && next == '/') { inBlockComment = false; i++; } continue; }
                if (inString) { if (c == '\\') i++; else if (c == '"') inString = false; continue; }
                if (inChar) { if (c == '\\') i++; else if (c == '\'') inChar = false; continue; }

                if (c == '/' && next == '/') { inLineComment = true; i++; continue; }
                if (c == '/' && next == '*') { inBlockComment = true; i++; continue; }
                if (c == '"') { inString = true; continue; }
                if (c == '\'') { inChar = true; continue; }

                if (c == '{') depth++;
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }
            return -1;
        }

        /// <summary>Index just after the last member token in the body (skipping trailing comments).</summary>
        private static int LastMemberEnd(string trimmedBody)
        {
            // Walk back over trailing line comments.
            var lines = trimmedBody.Split('\n');
            int offset = trimmedBody.Length;
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string line = lines[i];
                string stripped = Regex.Replace(line, @"//.*$", "").TrimEnd();
                if (stripped.TrimStart().Length > 0)
                {
                    int lineStart = trimmedBody.LastIndexOf(line, StringComparison.Ordinal);
                    return lineStart + stripped.Length;
                }
                offset -= line.Length + 1;
            }
            return trimmedBody.Length;
        }

        private static string ToAssetPath(string absolute)
        {
            absolute = absolute.Replace('\\', '/');
            string dataPath = Application.dataPath.Replace('\\', '/');
            return absolute.StartsWith(dataPath)
                ? "Assets" + absolute.Substring(dataPath.Length)
                : absolute;
        }
    }
}