using UnityEngine;

public class MainMenuCheatsOptionBehaviour : MonoBehaviour {
    /// <summary>
    /// Skin for the cheat dialog. Required, may not be null.
    /// </summary>
    public GUISkin Skin;

    private void Awake() {
        CheatInputDialog.HideDialog();
    }

    private void OnMouseUpAsButton() {
        CheatInputDialog.ShowDialog();
    }

    private void OnGUI() {
        if (Skin != null) {
            CheatInputDialog.DrawDialog(this.Skin);
        }
    }
}