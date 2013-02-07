using System;
using UnityEngine;

/// <summary>
/// Enables zooming in from a specified position to the start camera position
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(ArrowMovementCameraBehaviour))]
class CameraZoomStartController : MonoBehaviour {
    private MonoBehaviour cameraControllerScript;

    private Vector3 startPoint;
    private Quaternion startRot;
    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;

    private float startTime;

    /// <summary>
    /// Specifies the zoom start camera position
    /// </summary>
    public Vector3 StartCameraRotation = new Vector3();

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
    public float ZoomTime = 5;

    /// <summary>
    /// Specifies the speed used for zooming in to the correct camera rotation
    /// </summary>
    public float RotateTime = 5;

    void Awake() {
        this.cameraControllerScript = this.GetComponent<ArrowMovementCameraBehaviour>();
    }

    void Start() {
        HUD.Instance.EnableCountDown = false;

        this.cameraControllerScript.enabled = false;
        this.startTime = Time.time;

        // get start rotation/position
        this.targetCameraPosition = this.transform.position;
        this.targetCameraRotation = this.transform.rotation;

        // set new start state
        if (this.IsStartCameraPositionRelative) {
            this.transform.position += this.StartCameraPosition;
        } else {
            this.transform.position = this.StartCameraPosition;
        }
        this.transform.rotation = Quaternion.Euler(this.StartCameraRotation);

        this.startPoint = this.transform.position;
        this.startRot = this.transform.rotation;
    }


    void Update() {
        float timeDiff = Time.time - startTime;

        // smoothly set the new rotation
        Quaternion newRotation = Quaternion.Slerp(this.startRot, this.targetCameraRotation, timeDiff / this.RotateTime);
        this.transform.rotation = newRotation;

        // smoothly set the new position
        Vector3 newPosition = Vector3.Lerp(this.startPoint, this.targetCameraPosition, timeDiff / this.ZoomTime);
        this.transform.position = newPosition;

        // check if we're reached the position, and then enable control scripts
        bool positionReached = Vector3.Distance(this.transform.position, this.targetCameraPosition) < 1f;
        bool rotationReached = Quaternion.Angle(this.transform.rotation, this.targetCameraRotation) < 0.25f;

        if (positionReached && rotationReached) {
            this.cameraControllerScript.enabled = true;
            this.enabled = false;
            HUD.Instance.EnableCountDown = true;
        }
    }
}