using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class TouchDragCameraBehaviour : MonoBehaviour {

	////The target which the camera is set to look at from the start of the script.
	////This property changes the cameraLookAtTarget, which is private for safety reasons
	//public Vector3 cameraLookAtTargetStart = new Vector3();
	//private Vector3 cameraLookAtTarget = Vector3.zero;
	////The distance of the camera to the CameraLookAtTarget
	//public Vector3 CameraToTargetDistance = new Vector3(0f, 20f, -7.5f);

	private Vector3 OnMouseDownCursorPoint = new Vector3();
	private Vector3 OnMouseDownCameraPosition = new Vector3();

	public Vector2 BoundingBoxBottomLeft;
	public Vector2 BoundingBoxTopRight;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
		////Set CameraLookAtTargetStart the default if the cameralookattarget is set as vector3.zero
		//if(cameraLookAtTargetStart == Vector3.zero)
		//    cameraLookAtTargetStart = new Vector3(
		//        Camera.main.transform.position.x - CameraToTargetDistance.x,
		//        Camera.main.transform.position.y - CameraToTargetDistance.y,
		//        Camera.main.transform.position.z - CameraToTargetDistance.z);

		//cameraLookAtTarget = cameraLookAtTargetStart;

		//transform.position = new Vector3(
		//        cameraLookAtTarget.x + this.CameraToTargetDistance.x,
		//        cameraLookAtTarget.y + this.CameraToTargetDistance.y,
		//        cameraLookAtTarget.z + this.CameraToTargetDistance.z);

		//transform.LookAt(cameraLookAtTarget);
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () {

		if (Input.GetMouseButtonDown(0) && !MouseManager.TryAcquireLock(this)) {
			return;
		}

		if (Input.GetMouseButtonDown(0)) OnMouseDown();
		if (Input.GetMouseButton(0) && OnMouseDownCameraPosition != Vector3.zero) OnMouse();
		if (Input.GetMouseButtonUp(0)) OnMouseUp();

		float x = transform.position.x;
		float z = transform.position.z;

		if (transform.position.x < BoundingBoxBottomLeft.x) x = BoundingBoxBottomLeft.x;
		if (transform.position.x > BoundingBoxTopRight.x) x = BoundingBoxTopRight.x;
		if (transform.position.z < BoundingBoxBottomLeft.y) z = BoundingBoxBottomLeft.y;
		if (transform.position.z > BoundingBoxTopRight.y) z = BoundingBoxTopRight.y;

		transform.position = new Vector3(x, transform.position.y, z);

	}

	/// <summary>
	/// Called when MouseButton is pressed for the first time
	/// Creates an offset with which you can calculate the camera movement
	/// </summary>
	private void OnMouseDown() {
		OnMouseDownCursorPoint = Input.mousePosition;
		OnMouseDownCameraPosition = transform.position;
	}

	/// <summary>
	/// Called when MouseButton is held down
	/// Used to calculate the movement of the camera
	/// </summary>
	private void OnMouse() {
		transform.position = new Vector3(
			OnMouseDownCameraPosition.x + ((Input.mousePosition.x - OnMouseDownCursorPoint.x) * -0.1f),
			OnMouseDownCameraPosition.y,
			OnMouseDownCameraPosition.z + ((Input.mousePosition.y - OnMouseDownCursorPoint.y) * -0.1f));
	}

	/// <summary>
	/// Reset OnMouseDownCameraPosition, so it can't effect the following touchdown
	/// </summary>
	private void OnMouseUp() {
		OnMouseDownCameraPosition = Vector3.zero;
		
		if(MouseManager.CurrentLockOwner == this)
			MouseManager.ReleaseLock(this);
	}


    void OnDrawGizmosSelected() {
        // draw the bounding box for the camera
        // ... calculate center and size
        Vector3 center = (this.BoundingBoxTopRight / 2f) + this.BoundingBoxBottomLeft;
        Vector3 size = this.BoundingBoxTopRight - this.BoundingBoxBottomLeft;

        // ... actually the Y is the Z-axis, so swap it 
        center.z = center.y;
        size.z = size.y;

        // ... calculate y size by finding the terrain
        Terrain t = FindObjectOfType(typeof (Terrain)) as Terrain;
        if (t != null) {
            size.y = 50;  //this.transform.position.y - t.transform.position.y;
        } else {
            size.y = 50;
        }

        center.y = this.transform.position.y - (size.y / 2f);

        Gizmos.DrawWireCube(center, size);
        Gizmos.DrawWireCube(this.transform.position, new Vector3(6,6,6));

        // draw the rays for each of the sides
        // TODO
    }

    
}
