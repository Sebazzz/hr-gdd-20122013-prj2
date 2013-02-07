using System;
using UnityEngine;

/// <summary>
/// Attaches the camera to the clicked object
/// </summary>
public sealed class AttachCameraToClickedObjectBehaviour : MonoBehaviour {
    private Vector3 targetPosition;
    private Vector3 targetRotation;
    private Space targetSpace;

    public void Enable(Vector3 position, Vector3 rotation, Space space) {
        this.enabled = true;

        this.targetPosition = position;
        this.targetRotation = rotation;
        this.targetSpace = space;
    }

    void Update() {
        // detect clicked object to attach camera to
        if (Input.GetMouseButtonDown(0)) {
            Ray r = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(r, Mathf.Infinity, Layers.Default);
            
            if (hits != null) {
                foreach (RaycastHit hit in hits) {
                    // game object should not be static
                    GameObject obj = hit.collider.gameObject;

                    if (obj.isStatic) {
                        continue;
                    }

                    if (obj.tag == Tags.Trap ||
                        obj.tag == Tags.LevelBounds) {
                        continue;
                    }

                    this.AttachCameraTo(obj);
                }
            }
        }
    }

    private void AttachCameraTo(GameObject obj) {
        this.enabled = false;

        // set other as parent
        this.transform.parent = obj.transform;

        // set positional properties
        if (this.targetSpace == Space.World) {
            this.transform.position = this.targetPosition;

            if (!Single.IsNaN(this.targetRotation.x)) {
                this.transform.rotation = Quaternion.Euler(this.targetRotation);
            }
        } else {
            this.transform.localPosition = this.targetPosition;

            if (!Single.IsNaN(this.targetRotation.x)) {
                this.transform.localRotation = Quaternion.Euler(this.targetRotation);
            } else {
                Vector3 rot = this.transform.localRotation.eulerAngles;
                rot.y = 0;
                this.transform.localRotation = Quaternion.Euler(rot);
            }
        }

        // disable rotation constraints
        ArrowMovementCameraBehaviour cameraController = this.gameObject.GetComponent<ArrowMovementCameraBehaviour>();
        if (cameraController != null) {
            cameraController.MinimumRotation = 0;
            cameraController.MaximumRotation = 90;

            cameraController.RedetectScrollPointDistance();
        }
    }
}