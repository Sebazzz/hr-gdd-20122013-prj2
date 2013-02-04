using UnityEngine;

/// <summary>
/// Controller for cheats menu option
/// </summary>
public sealed class MainMenuCheatsOptionBehaviour : MonoBehaviour {
    /// <summary>
    /// Skin for the cheat dialog. Required, may not be null.
    /// </summary>
    public GUISkin Skin;

    private void Awake() {
        DialogController.HideDialogs();
    }

    private void OnMouseUpAsButton() {
        CheatInputDialog.ShowDialog();
    }

    private void OnGUI() {
        if (Skin != null) {
            DialogController.DrawDialogs(this.Skin);
        }
    }
}