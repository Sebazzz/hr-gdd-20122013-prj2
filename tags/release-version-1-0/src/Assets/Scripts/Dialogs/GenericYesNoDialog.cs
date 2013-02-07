using System;
using UnityEngine;

/// <summary>
/// Represents a generic dialog that shows an yes and no button and executes an delegate
/// </summary>
public static class GenericYesNoDialog {
    private static bool _ShowDialog;
    private static Rect _DialogRect;

    private static string _DialogTitle;
    private static string _DialogText;
    private static ButtonInfo _YesButton;
    private static ButtonInfo _NoButton;

    public static void ShowDialog(string title, string text, ButtonInfo yesButtonInfo, ButtonInfo noButtonInfo) {
        DialogController.HideDialogs();
        _ShowDialog = true;

        _DialogTitle = title;
        _DialogText = text;

        _YesButton = yesButtonInfo;
        _NoButton = noButtonInfo;

        const int width = 300;
        const int height = 200;
        _DialogRect = new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height);
    }

    public static void HideDialog() {
        _ShowDialog = false;

        _YesButton = default(ButtonInfo);
        _NoButton = default(ButtonInfo);
    }

    public static void DrawDialog(GUISkin skin) {
        if (_ShowDialog) {
            _DialogRect = GUI.Window(0, _DialogRect, (i) => DrawInsideDialog(i, skin), _DialogTitle, skin.window);
        }
    }

    private static void DrawInsideDialog(int id, GUISkin skin) {
        GUI.Label(new Rect(25, 25, 250, 130), _DialogText, skin.GetStyle("WindowText"));

        // yes button
        if (GUI.Button(new Rect(30, 160, 50, 30), String.Empty, skin.GetStyle("YesButton"))) {
            if (_YesButton.OnClickAction != null) {
                _YesButton.OnClickAction.Invoke();
            }

            HideDialog();
            return;
        }

        // no button
        if (GUI.Button(new Rect(220, 160, 50, 30), String.Empty, skin.GetStyle("NoButton"))) {
            if (_NoButton.OnClickAction != null) {
                _NoButton.OnClickAction.Invoke();
            }

            HideDialog();
        }
    }

    #region Nested type: ButtonInfo

    /// <summary>
    /// Represents informatie on a button
    /// </summary>
    public struct ButtonInfo {
        public readonly Action OnClickAction;
        public readonly string Text;

        public ButtonInfo(string text, Action onClickAction) {
            this.Text = text;
            this.OnClickAction = onClickAction;
        }
    }

    #endregion
}