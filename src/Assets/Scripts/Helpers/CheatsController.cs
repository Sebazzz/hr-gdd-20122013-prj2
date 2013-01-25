using UnityEngine;

/// <summary>
/// Helper script for processing various cheats and enable in-game cheats
/// </summary>
[RequireComponent(typeof(HUD))]
public sealed class CheatsController : MonoBehaviour {
    /// <summary>
    /// Specifies if the in-game cheat menu is enabled
    /// </summary>
    public static bool EnableInGameCheatsMenu = Debug.isDebugBuild;

    public static bool EnableLargeSheep = false;

    private void Awake() {
        Cheats.Dialog.HideDialog();
    }

    private void Start() {
        if (EnableLargeSheep) {
            SetLargeSheep();
        }
    }

    private static void SetLargeSheep() {
        const int scale = 4;

        // find all sheep and enlarge them
        GameObject[] sheepArray = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject sheep in sheepArray) {
            sheep.transform.localScale = new Vector3(scale, scale, scale);

            // increase Y because collider is bigger
            var sheepCollider = sheep.GetComponent<Collider>();
            float ySize = sheepCollider.bounds.size.y;

            Vector3 pos = sheep.transform.position;
            pos.y += (ySize*scale)/2f;
            sheep.transform.position = pos;
        }
    }

    private void OnGUI() {
        // get the GUI style from the HUD script
        GUISkin guiStyle = this.GetComponent<HUD>().skin;

        int x = Screen.width - 50;
        int y = Screen.height - 25;

        if (GUI.Button(new Rect(x, y, 50, 25), "Cheats", guiStyle.GetStyle("button"))) {
            Cheats.Dialog.ShowDialog();
        }

        // draw dialog
        Cheats.Dialog.DrawDialog(guiStyle);
    }
}