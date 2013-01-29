using UnityEngine;

/// <summary>
/// Helper script for processing various cheats and enable in-game cheats
/// </summary>
[RequireComponent(typeof(HUD))]
public sealed class CheatsController : MonoBehaviour {
    private static bool _HasSetEnableInGameCheatsMenuDefaultValue = false;

    /// <summary>
    /// Specifies if the rotation lock on the sheep <see cref="Rigidbody"/> is disabled
    /// </summary>
    public static bool EnableSheepRotationLock = true;

    /// <summary>
    /// Specifies if the in-game cheat menu is enabled
    /// </summary>
    public static bool EnableInGameCheatsMenu = false;

    public static bool EnableLargeSheep = false;

    public static bool TerrainBounce = false;

    private void Awake() {
        Cheats.InputDialog.HideDialog();

        if (!_HasSetEnableInGameCheatsMenuDefaultValue) {
            EnableInGameCheatsMenu = Debug.isDebugBuild;
            _HasSetEnableInGameCheatsMenuDefaultValue = true;
        }
    }

    private void Start() {
        if (EnableLargeSheep) {
            SetLargeSheep();
        }

        if (!EnableSheepRotationLock) {
            RemoveSheepRigidbodyRotationConstraint();
        }
    }

    private void Update() {
        if (EnableInGameCheatsMenu && Input.GetKeyUp(KeyCode.BackQuote)) {
            Cheats.InputDialog.ShowDialog();
        }
    }

    private static void RemoveSheepRigidbodyRotationConstraint() {
        GameObject[] sheepArray = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject sheep in sheepArray) {
            Rigidbody rb = sheep.GetComponent<Rigidbody>();

            if (rb == null) {
                continue;
            }

            rb.constraints = RigidbodyConstraints.None;
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

        const int width = 60;
        const int height = 30;

        int x = Screen.width - width;
        int y = Screen.height - height;

        if (GUI.Button(new Rect(x, y, width, height), "Cheats", guiStyle.GetStyle("button"))) {
            Cheats.InputDialog.ShowDialog();
        }

        // draw dialog
        Cheats.InputDialog.DrawDialog(guiStyle);
    }
}