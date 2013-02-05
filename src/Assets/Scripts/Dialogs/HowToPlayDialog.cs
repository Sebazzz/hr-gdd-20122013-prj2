using System;
using UnityEngine;

/// <summary>
/// Represents a generic dialog that shows an yes and no button and executes an delegate
/// </summary>
public static class HowToPlayDialog {
    private static bool _ShowDialog;


    private static Texture2D _HowToTexture;

    private static ButtonInfo _ContinueButton;

    public static void ShowDialog(ButtonInfo buttonInfo) {
        DialogController.HideDialogs();
        _ShowDialog = true;

        _HowToTexture = (Texture2D)Resources.Load("Hud/uitleg");

        _ContinueButton = buttonInfo;

    }

    public static void HideDialog() {
        _ShowDialog = false;

        _ContinueButton = default(ButtonInfo);
    }

    public static void DrawDialog(GUISkin skin) {
        if (_ShowDialog) {
            DrawInsideDialog(1, skin);
            //_DialogRect = GUI.Window(0, _DialogRect, (i) => DrawInsideDialog(i, skin), "", skin.window);
        }
    }

    private static void DrawInsideDialog(int id, GUISkin skin) {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _HowToTexture);

        // yes button
        if (GUI.Button(new Rect(Screen.width - 60, 10, 50, 30), String.Empty, skin.GetStyle("ContinueButton"))) {
            if (_ContinueButton.OnClickAction != null) {
                _ContinueButton.OnClickAction.Invoke();
            }

            HideDialog();
            return;
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