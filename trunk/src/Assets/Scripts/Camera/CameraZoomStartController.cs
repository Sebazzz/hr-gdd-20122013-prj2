using System;
using UnityEngine;

/// <summary>
/// Enables zooming in from a specified position to the start camera position
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(ArrowMovementCameraBehaviour))]
class CameraZoomStartController : MonoBehaviour {
    private MonoBehaviour cameraControllerScript;
    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;

    /// <summary>
    /// Specifies the zoom start camera position
    /// </summary>
    public Vector3 StartCameraRotation;

    /// <summary>
    /// Specifies the start camera position to zoom in from
    /// </summary>
    public Vector3 StartCameraPosition = new Vector3(0,15,0);

    /// <summary>
    /// Specifies if the <see cref="StartCameraPosition"/> is relative to the current camera position
    /// </summary>
    public bool IsStartCameraPositionRelative = true;

    /// <summary>
    /// Specifies the speed used for zooming in to the correct camera position
    /// </summary>
    public float ZoomSpeed = 1;

    /// <summary>
    /// Specifies the speed used for zooming in to the correct camera rotation
    /// </summary>
    public float RotateSpeed = 1;


    void Start() {
        // get the start state
        this.cameraControllerScript = this.GetComponent<ArrowMovementCameraBehaviour>();
        this.cameraControllerScript.enabled = false;

        this.targetCameraPosition = this.transform.position;
        this.targetCameraRotation = this.transform.rotation;

        // set new start state
        if (this.IsStartCameraPositionRelative) {
            this.transform.position += this.StartCameraPosition;
        } else {
            this.transform.position = this.StartCameraPosition;
        }
        this.transform.rotation = Quaternion.Euler(this.StartCameraRotation);
    }


    void Update() {
        // smoothly set the new rotation
        float rotationSpeed = this.RotateSpeed * Time.deltaTime;
        Quaternion newRotation = Quaternion.Slerp(this.transform.rotation, this.targetCameraRotation, rotationSpeed);
        this.transform.rotation = newRotation;

        // smoothly set the new position
        float zoomSpeed = this.ZoomSpeed * Time.deltaTime;
        Vector3 newPosition = Vector3.Lerp(this.transform.position, this.targetCameraPosition, zoomSpeed);

        // ... if the distance for the rotation is too small, we speed it up a little bit because it takes too long otherwise
        float diffDistance = Vector3.Distance(newPosition, this.transform.position);
        if (diffDistance < 0.5f) {
            newPosition = Vector3.Slerp(this.transform.position, this.targetCameraPosition, (1f / diffDistance) * 0.5f * zoomSpeed);
        }
        this.transform.position = newPosition;

        //this.transform.position = Vector3.MoveTowards(this.transform.position, this.targetCameraPosition, this.ZoomSpeed);

        // check if we're reached the position, and then enable control scripts
        bool positionReached = Vector3.Distance(this.transform.position, this.targetCameraPosition) < 1f;
        bool rotationReached = Quaternion.Angle(this.transform.rotation, this.targetCameraRotation) < 2f;
        
        if (positionReached && rotationReached) {
            this.cameraControllerScript.enabled = true;
            this.enabled = false;
        }
    }
}