using UnityEngine;

public class MainMenuCheatsOptionBehaviour : MonoBehaviour {
    /// <summary>
    /// Skin for the cheat dialog. Required, may not be null.
    /// </summary>
    public GUISkin Skin;

    private void Awake() {
        Cheats.Dialog.HideDialog();
    }

    private void OnMouseUpAsButton() {
        Cheats.Dialog.ShowDialog();
    }

    private void OnGUI() {
        if (Skin != null) {
            Cheats.Dialog.DrawDialog(this.Skin);
        }
    }
}