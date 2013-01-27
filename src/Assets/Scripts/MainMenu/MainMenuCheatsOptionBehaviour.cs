using UnityEngine;

public class MainMenuCheatsOptionBehaviour : MonoBehaviour {
    /// <summary>
    /// Skin for the cheat dialog. Required, may not be null.
    /// </summary>
    public GUISkin Skin;

    private void Awake() {
        Cheats.InputDialog.HideDialog();
    }

    private void OnMouseUpAsButton() {
        Cheats.InputDialog.ShowDialog();
    }

    private void OnGUI() {
        if (Skin != null) {
            Cheats.InputDialog.DrawDialog(this.Skin);
        }
    }
}