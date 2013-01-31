using System;
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
    private static string[] _DialogBodyTextColumn1;
    private static string[] _DialogBodyTextColumn2;

    /// <summary>
    /// Enables showing of the dialog
    /// </summary>
    public static void ShowDialog(string title, string[] bodyColumn1, string[] bodyColumn2, string bodyStyleName) {
        _ShowDialog = true;

        const int width = 600;
        const int height = 400;
        _DialogRect = new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height);

        _DialogBodyTitle = title;
        _DialogBodyTextColumn1 = bodyColumn1;
        _DialogBodyTextColumn2 = bodyColumn2;

        if (bodyColumn1.Length != bodyColumn2.Length) {
            throw new Exception("Both column text arrays must have same length!");
        }
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
        for (int i=0;i<_DialogBodyTextColumn1.Length&&i<_DialogBodyTextColumn2.Length;i++) {
            string col1Text = _DialogBodyTextColumn1[i];
            string col2Text = _DialogBodyTextColumn2[i];

            GUILayout.BeginHorizontal();

            bool createSecondColumn = col2Text != null;
            if (createSecondColumn) {
                GUILayout.Label(col1Text, textStyle, GUILayout.Width(225));

                GUILayout.BeginVertical();
                GUILayout.Label(col2Text, textStyle);
                GUILayout.EndVertical();
            } else {
                GUILayout.Label(col1Text, textFatStyle);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
            
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

}