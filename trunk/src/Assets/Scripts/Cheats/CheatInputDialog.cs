using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Helper class for showing a cheat dialog
/// </summary>
public static class CheatInputDialog {
    private static bool _ShowDialog;
    private static Rect _DialogRect;
    private static string _EnteredCheat = String.Empty;

    /// <summary>
    /// Enables showing of the dialog
    /// </summary>
    public static void ShowDialog() {
        _EnteredCheat = String.Empty;
        _ShowDialog = true;

        const int width = 300;
        const int height = 100;
        _DialogRect = new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height);

        CheatReferenceDialog.HideDialog();
        CheatNotificationDialog.HideDialog();
    }

    /// <summary>
    /// Disables showing of the dialog
    /// </summary>
    public static void HideDialog() {
        _ShowDialog = false;

        CheatReferenceDialog.HideDialog();
        CheatNotificationDialog.HideDialog();
    }

    /// <summary>
    /// Draws the dialog. Call in <c>OnGUI</c>.
    /// </summary>
    /// <param name="skin"></param>
    public static void DrawDialog(GUISkin skin) {
        if (_ShowDialog) {
            _DialogRect = GUILayout.Window(0, _DialogRect, i => DrawInsideDialog(i, skin), "Cheats",
                                           skin.GetStyle("window"));
        }

        CheatReferenceDialog.DrawDialog(skin);
        CheatNotificationDialog.DrawDialog(skin);
    }

    private static void DrawInsideDialog(int dialogId, GUISkin skin) {
        // label
        GUILayout.BeginHorizontal();
        GUILayout.Label("Enter cheat: ", skin.GetStyle("label"));
        GUILayout.EndHorizontal();

        // text field
        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("CheatInputBox");
        _EnteredCheat = GUILayout.TextField(_EnteredCheat, 60, skin.GetStyle("textfield")).TrimStart('`');
        GUILayout.EndHorizontal();

        bool cheatEnterKeyPressed = _EnteredCheat.Length > 0 &&
                                    (_EnteredCheat[_EnteredCheat.Length - 1] == '\n' ||
                                     _EnteredCheat[_EnteredCheat.Length - 1] == '\r');

        // button
        GUILayout.BeginHorizontal(GUILayout.Width(50));
        if (GUILayout.Button("Apply", skin.GetStyle("button")) || cheatEnterKeyPressed) {
            _ShowDialog = false;

            if (String.IsNullOrEmpty(_EnteredCheat)) {
                _EnteredCheat = String.Empty;
            }
            else {
                _EnteredCheat = _EnteredCheat.TrimEnd('\n', ' ', '\r');
                CheatService.ExecuteRawCommand(_EnteredCheat);
            }
        }
        GUILayout.EndHorizontal();

        // set focus to cheat input box
        if (GUI.GetNameOfFocusedControl() == String.Empty) {
            GUI.FocusControl("CheatInputBox");
        }

    }
}