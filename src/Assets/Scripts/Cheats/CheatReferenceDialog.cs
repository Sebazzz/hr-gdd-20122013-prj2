using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

/// <summary>
/// Represents a dialog that shows in a two-column grid a overview of the contents specified
/// </summary>
internal static class CheatReferenceDialog {
    private const string TextStyle = "CheatDialogText";
    private const string TextFatStyle = "CheatDialogTextFat";

    private static Rect _DialogRect;
    private static bool _ShowDialog;
    private static Vector2 _ScrollPosition;

    private static string _DialogBodyTitle;
    private static Dictionary<string, List<CheatCommandDescriptor>> _CheatCommandsByCategory;
    private static IEnumerable<CheatVariabeleDescriptor> _CheatVariables;

    /// <summary>
    /// Enables showing of the dialog
    /// </summary>
    public static void ShowDialog(string title, Dictionary<string, List<CheatCommandDescriptor>> cheatCommandsByCategory, IEnumerable<CheatVariabeleDescriptor> cheatVariables, string bodyStyleName) {
        _ShowDialog = true;

        const int width = 900;
        const int height = 400;
        _DialogRect = new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height);

        _DialogBodyTitle = title;
        _CheatCommandsByCategory = cheatCommandsByCategory;
        _CheatVariables = cheatVariables;
    }

    /// <summary>
    /// Disables showing of the dialog
    /// </summary>
    public static void HideDialog() {
        _ShowDialog = false;
    }

    /// <summary>
    /// Draws the dialog. Call in <c>OnGUI</c>.
    /// </summary>
    /// <param name="skin"></param>
    public static void DrawDialog(GUISkin skin) {
        if (_ShowDialog) {
            _DialogRect = GUILayout.Window(0, _DialogRect, i => DrawInsideDialog(i, skin), _DialogBodyTitle,
                                           skin.GetStyle("window"));
        }
    }

    private static void DrawInsideDialog(int dialogId, GUISkin skin) {
        GUIStyle textStyle = skin.GetStyle(TextStyle);
        GUIStyle textFatStyle = skin.GetStyle(TextFatStyle);

        _ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition, false, true, skin.GetStyle("horizontalscrollbar"),
                                                    skin.GetStyle("verticalscrollbar"), skin.GetStyle("scrollview"), GUILayout.ExpandWidth(true), GUILayout.Height(350));

        // list cheats per category
        foreach (KeyValuePair<string, List<CheatCommandDescriptor>> cheatsPerCategory in _CheatCommandsByCategory) {
            // cheat category title
            CreateRow(cheatsPerCategory.Key + ":", textFatStyle);

            // list cheats
            foreach (CheatCommandDescriptor commandDescriptor in cheatsPerCategory.Value) {
                // command formatted with arguments
                var format = new StringBuilder();
                format.Append(commandDescriptor.Name);

                foreach (ParameterInfo parameterInfo in commandDescriptor.Parameters) {
                    format.AppendFormat(" <{0}>", parameterInfo.Name);
                }

                CreateRow(format.ToString(), commandDescriptor.Description, textStyle);
            }

            // empty space
            CreateRow("   ", textStyle);
        }

        // list variabeles
        CreateRow("  ", textStyle);
        CreateRow("Variables that can be set:", textFatStyle);

        foreach (CheatVariabeleDescriptor variabeleDescriptor in _CheatVariables) {
            string name = variabeleDescriptor.Name;

            Type fieldType = variabeleDescriptor.FieldInfo.FieldType;
            string typeName = fieldType.Name.ToLowerInvariant();

            if (fieldType == typeof(double) || fieldType == typeof(float)) {
                typeName = "decimal";
            } else if (fieldType == typeof(int) || fieldType == typeof(long)) {
                typeName = "integer";
            }

            string format = String.Format("{0} ({1})", name, typeName);
            CreateRow(format, variabeleDescriptor.Description, textStyle);
        }

        CreateRow("   ", textStyle);
        CreateRow("Note: Some cheats may only be applied after a level reload.", textStyle);

        GUILayout.EndScrollView();
        
        // close button
        GUILayout.BeginHorizontal(GUILayout.Width(50));
        GUI.SetNextControlName("CloseButton");
        if (GUILayout.Button("Close", skin.GetStyle("button"))) {
            _ShowDialog = false;
        }
            
        GUILayout.EndHorizontal();

        // set focus to close button
        if (GUI.GetNameOfFocusedControl() == String.Empty) {
            GUI.FocusControl("CloseButton");
        }
    }

    private static void CreateRow(string contents, GUIStyle style) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(contents, style);
        GUILayout.EndHorizontal();
    }

    private static void CreateRow(string column1Contents, string column2Contents, GUIStyle style) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(column1Contents, style, GUILayout.Width(350));

        GUILayout.BeginVertical();
        GUILayout.Label(column2Contents, style);
        GUILayout.EndVertical();


        GUILayout.EndHorizontal();
    }
}