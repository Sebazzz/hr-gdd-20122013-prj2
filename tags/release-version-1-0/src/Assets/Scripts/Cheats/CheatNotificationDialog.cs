using System;
using UnityEngine;

/// <summary>
/// Represents a simple text dialog with notification text. Note: only to be used by the <see cref="CheatInputDialog"/> and related classes.
/// </summary>
internal static class CheatNotificationDialog {
    private const string TextStyle = "CheatDialogText";
    private const string TextFatStyle = "CheatDialogTextFat";

    private static Rect _DialogRect;
    private static bool _ShowDialog;

    private static string _DialogBodyTitle;
    private static string _DialogBodyText;

    /// <summary>
    /// Enables showing of the dialog
    /// </summary>
    public static void ShowDialog(string title, string text) {
        DialogController.HideDialogs();

        _ShowDialog = true;

        const int width = 300;
        const int height = 100;
        _DialogRect = new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height);

        _DialogBodyTitle = title;
        _DialogBodyText = text;
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

        GUILayout.BeginHorizontal();

        GUILayout.Label(_DialogBodyText, textStyle);

        GUILayout.EndHorizontal();

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