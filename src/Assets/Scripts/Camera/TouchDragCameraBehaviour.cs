using UnityEngine;
using System.Collections;

public class TouchDragCameraBehaviour : MonoBehaviour {

	//The target which the camera is set to look at from the start of the script.
	//This property changes the cameraLookAtTarget, which is private for safety reasons
	public Vector3 cameraLookAtTargetStart = new Vector3();
	private Vector3 cameraLookAtTarget = Vector3.zero;
	//The distance of the camera to the CameraLookAtTarget
	public Vector3 CameraToTargetDistance = new Vector3(0f, 20f, -7.5f);

	private Vector3 OnMouseDownCursorPoint = new Vector3();
	private Vector3 OnMouseDownCameraPosition = new Vector3();

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
		//Set CameraLookAtTargetStart the default if the cameralookattarget is set as vector3.zero
		if(cameraLookAtTargetStart == Vector3.zero)
			cameraLookAtTargetStart = new Vector3(
				Camera.main.transform.position.x - CameraToTargetDistance.x,
				Camera.main.transform.position.y - CameraToTargetDistance.y,
				Camera.main.transform.position.z - CameraToTargetDistance.z);

		cameraLookAtTarget = cameraLookAtTargetStart;

		transform.position = new Vector3(
				cameraLookAtTarget.x + this.CameraToTargetDistance.x,
				cameraLookAtTarget.y + this.CameraToTargetDistance.y,
				cameraLookAtTarget.z + this.CameraToTargetDistance.z);

		transform.LookAt(cameraLookAtTarget);
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit = new RaycastHit();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				foreach (GameObject go in GameObject.FindGameObjectsWithTag(Tags.Shepherd)) {
					if (hit.collider.gameObject == go.gameObject) return;
				}
			}
		}


		if (Input.GetMouseButtonDown(0)) OnMouseDown();
		if (Input.GetMouseButton(0) && OnMouseDownCameraPosition != Vector3.zero) OnMouse();
		if (Input.GetMouseButtonUp(0)) OnMouseUp();
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
	}

	/// <summary>
	/// Returns the cameraLookAtTarget Vector 3, so other objects can have the center point of the camera
	/// </summary>
	public Vector3 GetCameraLookAtTarget() {
		return cameraLookAtTarget;
	}
}
