using UnityEngine;

// Cheats implementation: Functional
public static partial class CheatImplementation {
    [CheatCommand("EndlessCameraMovement", CheatCategory.DebuggingHelpers)]
    public static void DisableCameraMovementBounds() {
        var cameraScript = Camera.mainCamera.GetComponent<ArrowMovementCameraBehaviour>();

        if (cameraScript != null) {
            cameraScript.BoundingBoxBottomLeft = new Vector2(-1000, -1000);
            cameraScript.BoundingBoxTopRight = new Vector2(1000, 1000);
        }
    }

    [CheatCommand("ZoomCamera", CheatCategory.DebuggingHelpers)]
    public static void ZoomTheCameraInOrOutByTheSpecifiedAmount(float amount) {
        Camera c = Camera.mainCamera;

        Vector3 delta = c.transform.TransformDirection(Vector3.forward*amount); 
        c.transform.Translate(delta, Space.World);
    }


    [CheatCommand("ControllableSheep", CheatCategory.DebuggingHelpers)]
    public static void EnablesSheepToBeControlledByArrowKeysOptionallyDisablingControlHelperEffects(bool disableControlEffects) {
        // find dog marker
        GameObject sourceDog = GameObject.FindGameObjectWithTag(Tags.Shepherd);
        GameObject selectionMarker = null;

        if (sourceDog != null) {
            // get the projector
            foreach (Transform tr in sourceDog.transform) {
                if (tr.gameObject.name == "SelectionProjector") {
                    selectionMarker = tr.gameObject;
                    break;
                }
            }
        }


        // disable dog effects
        GameObject[] dogs = GameObject.FindGameObjectsWithTag(Tags.Shepherd);

        foreach (GameObject dog in dogs) {
            RepelBehaviour repeller = dog.GetComponent<RepelBehaviour>();

            if (repeller != null) {
                repeller.enabled = !disableControlEffects;
            }
        }

        // attach the controlling to the sheep
        GameObject[] sheepArr = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject sheep in sheepArr) {
            CheatControlSheepByArrowKeysBehaviour c = sheep.AddComponent<CheatControlSheepByArrowKeysBehaviour>();
            c.SetMarker(selectionMarker);

            MagneticBehaviour magnet = sheep.GetComponent<MagneticBehaviour>();
            if (magnet != null) {
                magnet.enabled = !disableControlEffects;
            }
        }

        CheatNotificationDialog.ShowDialog("Instruction", "Control the sheep using the JIKL keys (similar to WSAD). Select an sheep using the middle mouse button, unselect it by clicking again.");
    }

}