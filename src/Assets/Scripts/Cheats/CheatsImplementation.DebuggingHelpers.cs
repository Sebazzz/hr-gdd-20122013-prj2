using UnityEngine;

// Cheats implementation: Functional
public static partial class CheatsImplementation {
    [CheatCommand("EndlessCameraMovement", CheatCategory.DebuggingHelpers)]
    public static void DisableCameraMovementBounds() {
        var cameraScript = Camera.mainCamera.GetComponent<ArrowMovementCameraBehaviour>();

        if (cameraScript != null) {
            cameraScript.BoundingBoxBottomLeft = new Vector2(-1000, -1000);
            cameraScript.BoundingBoxTopRight = new Vector2(1000, 1000);
        }
    }
}