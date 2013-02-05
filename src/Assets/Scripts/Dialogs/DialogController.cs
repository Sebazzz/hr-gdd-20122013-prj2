using UnityEngine;

/// <summary>
/// Central class for controlling and drawing all dialogs
/// </summary>
public static class DialogController {
    public static void HideDialogs() {
        CheatReferenceDialog.HideDialog();
        CheatInputDialog.HideDialog();
        CheatNotificationDialog.HideDialog();

        GenericYesNoDialog.HideDialog();
        GameOverDialog.HideDialog();
        GameScoreDialog.HideDialog();

        HowToPlayDialog.HideDialog();
    }

    public static void DrawDialogs(GUISkin skin) {
        CheatInputDialog.DrawDialog(skin);
        CheatReferenceDialog.DrawDialog(skin);
        CheatNotificationDialog.DrawDialog(skin);

        GenericYesNoDialog.DrawDialog(skin);
        GameOverDialog.DrawDialog(skin);
        GameScoreDialog.DrawDialog(skin);

        HowToPlayDialog.DrawDialog(skin);
    }
}