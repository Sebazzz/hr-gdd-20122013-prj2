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

    [CheatCommand("SetCameraMode", CheatCategory.DebuggingHelpers)]
    public static void SetCameraToIsometricOrPerspectiveModeUsingTheSpecifiedSizeOrFieldOfView(CameraMode cameraMode, float param) {
        Camera.mainCamera.isOrthoGraphic = cameraMode == CameraMode.Iso;
        
        if (Camera.mainCamera.isOrthoGraphic) {
            Camera.mainCamera.orthographicSize = param;
        } else {
            Camera.mainCamera.fieldOfView = param;
        }
    }

    public enum CameraMode {
        Fov = 0,
        Perspective = 0,
        Perspect=0,
        Spec=0,
        Iso=1,
        Isometric=1,
        Sims=1
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
            ControlledByKeysBehaviour c = sheep.AddComponent<ControlledByKeysBehaviour>();
            c.SetMarker(selectionMarker);
            c.JumpForce = 1f;

            MagneticBehaviour magnet = sheep.GetComponent<MagneticBehaviour>();
            if (magnet != null) {
                magnet.enabled = !disableControlEffects;
            }
        }

        CheatNotificationDialog.ShowDialog(
            "Instructions", 
            "Control the sheep using the JIKL keys (similar to WSAD). Press U or O to strafe. Press space to jump. " +
            "Select an sheep using the middle mouse button, unselect it by clicking the middle mouse button again.");
    }

    [CheatCommand("KeyboardControllableDogs", CheatCategory.DebuggingHelpers)]
    public static void EnablesDogsToBeControlledByArrowKeys() {
        // attach the controlling script to the dogs
        GameObject[] dogs = GameObject.FindGameObjectsWithTag(Tags.Shepherd);

        foreach (GameObject dog in dogs) {
            // get the projector
            GameObject selectionMarker = null;
            foreach (Transform tr in dog.transform) {
                if (tr.gameObject.name == "SelectionProjector") {
                    selectionMarker = tr.gameObject;
                    break;
                }
            }

            // add component
            ControlledByKeysBehaviour c = dog.AddComponent<ControlledByKeysBehaviour>();
            c.SetMarker(selectionMarker);

            if (selectionMarker == null) {
                continue;
            }

            // destroy original marker
            selectionMarker.transform.parent = null;
            Object.Destroy(selectionMarker);

            // disable control scripts
            MonoBehaviour controlScript = dog.GetComponent<ControlHerderBehaviour>();
            if (controlScript != null) {
                controlScript.enabled = false;
            }

            controlScript = dog.GetComponent<HerderLoopBehaviour>();
            if (controlScript != null) {
                controlScript.enabled = false;
            }
        }

        CheatNotificationDialog.ShowDialog(
            "Instructions",
            "Control the dog using the JIKL keys (similar to WSAD). Press U or O to strafe. Press space to jump. " +
            "Select an dog using the middle mouse button, unselect it by clicking the middle mouse button again.");
    }

}