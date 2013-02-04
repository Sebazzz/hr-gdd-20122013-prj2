using System;
using UnityEngine;

/// <summary>
/// Represents the game over dialog
/// </summary>
public static class GameOverDialog {
    private static bool _ShowDialog;
    private static string _DialogText;
    private static Rect _DialogRect;

    public static void ShowDialog(string bodyText) {
        DialogController.HideDialogs();

        _DialogText = bodyText;
        _ShowDialog = true;

        const int width = 300;
        const int height = 200;
        _DialogRect = new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height);
    }

    public static void HideDialog() {
        _ShowDialog = false;
    }

    public static void DrawDialog(GUISkin skin) {
        if (_ShowDialog) {
            _DialogRect = GUI.Window(0, _DialogRect, i => DrawInsideDialog(i, skin), String.Empty, skin.window);
        }
    }

    private static void DrawInsideDialog(int id, GUISkin skin) {
        GUI.Label(new Rect(25, 25, 250, 130), _DialogText, skin.GetStyle("WindowText"));

        if (GUI.Button(new Rect(150 - 50, 150, 40, 40), "", skin.GetStyle("MenuScoreButton"))) {
            AsyncSceneLoader.Load(Scenes.MainMenu);

            HideDialog();

            return;
        }

        if (GUI.Button(new Rect(150 + 10, 150, 40, 40), "", skin.GetStyle("RestartScoreButton"))) {
            AsyncSceneLoader.Load(Application.loadedLevelName);

            HideDialog();
        }
    }
}