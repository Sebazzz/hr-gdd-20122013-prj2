using System;
using UnityEngine;

/// <summary>
/// Represents a dialog that shows the game score
/// </summary>
public static class GameScoreDialog {
    private const float AnimationSpeedModifier = 0.1f;
    private static bool _HasLoadedTextures;
    private static bool _ShowDialog;

    private static Rect _DialogRect;
    private static MainMenuReturnToLevelSelectBehaviour.SelectedWorld _CurrentWorld;
    private static bool _HasMaxScore;
    private static bool _HasMinScore;
    private static bool _HasTime;

    private static Texture2D _TimeScoreTexture;
    private static Texture2D _TimeScoreCheckedTexture;
    private static Texture2D _MinScoreTexture;
    private static Texture2D _MinScoreCheckedTexture;
    private static Texture2D _MaxScoreTexture;
    private static Texture2D _MaxScoreCheckedTexture;

    private static readonly Rect MinScoreTextureTargetRect = new Rect(150 - 90 - 40, 60, 80, 80);
    private static readonly Rect MinScoreTextureStartRect = new Rect(150 - 90 - 40, 60, 0, 0);
    private static Rect _MinScoreTextureRect;

    private static readonly Rect MaxScoreTextureTargetRect = new Rect(150 - 40, 60, 80, 80);
    private static readonly Rect MaxScoreTextureStartRect = new Rect(150 - 0, 60, 0, 0);
    private static Rect _MaxScoreTextureRect;

    private static readonly Rect TimeScoreTextureTargetRect = new Rect(150 + 90 - 40, 60, 80, 80);
    private static readonly Rect TimeScoreTextureStartRect = new Rect(150 + 90 + 40, 60, 0, 0);
    private static Rect _TimeScoreTextureRect;

    public static void ShowDialog(MainMenuReturnToLevelSelectBehaviour.SelectedWorld currentWorld, bool hasMinScore,
                                  bool hasMaxScore, bool hasTime) {
        if (!_HasLoadedTextures) {
            LoadResources();
        }

        DialogController.HideDialogs();
        _ShowDialog = true;
        _CurrentWorld = currentWorld;

        _MinScoreTextureRect = MinScoreTextureStartRect;
        _MaxScoreTextureRect = MaxScoreTextureStartRect;
        _TimeScoreTextureRect = TimeScoreTextureStartRect;

        _HasMinScore = hasMinScore;
        _HasMaxScore = hasMaxScore;
        _HasTime = hasTime;

        const int width = 300;
        const int height = 200;
        _DialogRect = new Rect(Screen.width/2 - (width/2), Screen.height/2 - (height/2), width, height);
    }

    private static void LoadResources() {
        _TimeScoreTexture = Resources.Load("Hud/time") as Texture2D;
        _TimeScoreCheckedTexture = Resources.Load("Hud/time-checked") as Texture2D;
        _MinScoreTexture = Resources.Load("Hud/min_sheep") as Texture2D;
        _MinScoreCheckedTexture = Resources.Load("Hud/min_sheep-checked") as Texture2D;
        _MaxScoreTexture = Resources.Load("Hud/max_sheep") as Texture2D;
        _MaxScoreCheckedTexture = Resources.Load("Hud/max_sheep-checked") as Texture2D;

        _HasLoadedTextures = true;
    }

    public static void HideDialog() {
        _ShowDialog = false;
    }

    public static void DrawDialog(GUISkin skin) {
        if (_ShowDialog) {
            _DialogRect = GUI.Window(0, _DialogRect, i => DrawInnerDialog(i, skin), String.Empty, skin.window);
        }
    }

    private static void DrawInnerDialog(int id, GUISkin skin) {
        GUI.Label(new Rect(25, 25, 250, 130), "Level Done", skin.GetStyle("WindowText"));

        // get the correct textures 
        Texture2D minT = _MinScoreTexture;
        if (_HasMinScore) {
            minT = _MinScoreCheckedTexture;
        }

        Texture2D maxT = _MaxScoreTexture;
        if (_HasMaxScore) {
            maxT = _MaxScoreCheckedTexture;
        }

        Texture2D maxTimeT = _TimeScoreTexture;
        if (_HasTime) {
            maxTimeT = _TimeScoreCheckedTexture;
        }

        // draw the textures with animation
        _MinScoreTextureRect = AnimateRect(_MinScoreTextureRect, MinScoreTextureTargetRect, AnimationSpeedModifier);
        GUI.DrawTexture(_MinScoreTextureRect, minT, ScaleMode.StretchToFill, true, 0);

        _MaxScoreTextureRect = AnimateRect(_MaxScoreTextureRect, MaxScoreTextureTargetRect, AnimationSpeedModifier);
        GUI.DrawTexture(_MaxScoreTextureRect, maxT, ScaleMode.StretchToFill, true, 0);

        _TimeScoreTextureRect = AnimateRect(_TimeScoreTextureRect, TimeScoreTextureTargetRect, AnimationSpeedModifier);
        GUI.DrawTexture(_TimeScoreTextureRect, maxTimeT, ScaleMode.StretchToFill, true, 0);


        if (GUI.Button(new Rect(150 - 50 - 20, 150, 40, 40), String.Empty, skin.GetStyle("MenuScoreButton"))) {
            MainMenuReturnToLevelSelectBehaviour.selectedWorld = MainMenuReturnToLevelSelectBehaviour.SelectedWorld.None;
            AsyncSceneLoader.Load(Scenes.MainMenu);

            HideDialog();
            return;
        }

        if (GUI.Button(new Rect(150 - 20, 150, 40, 40), String.Empty, skin.GetStyle("RestartScoreButton"))) {
            AsyncSceneLoader.Load(Application.loadedLevelName);
            HideDialog();
            return;
        }

        if (GUI.Button(new Rect(150 + 50 - 20, 150, 40, 40), String.Empty, skin.GetStyle("NextScoreButton"))) {
            MainMenuReturnToLevelSelectBehaviour.selectedWorld = _CurrentWorld;

            AsyncSceneLoader.Load(Scenes.MainMenu);
            HideDialog();
        }
    }

    private static Rect AnimateRect(Rect current, Rect target, float t) {
        var result = new Rect();
        result.xMin = Mathf.Lerp(current.xMin, target.xMin, t);
        result.yMin = Mathf.Lerp(current.yMin, target.yMin, t);
        result.width = Mathf.Lerp(current.width, target.width, t);
        result.height = Mathf.Lerp(current.height, target.height, t);

        return result;
    }
}