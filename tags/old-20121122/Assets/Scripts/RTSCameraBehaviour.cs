using UnityEngine;
using System.Collections;

public class RTSCameraBehaviour : MonoBehaviour {
    public Transform cameraTransform;
    public float panSpeed = 20;
	public float zoomSpeed = 25;
	public float cameraAngle = 45;
	private enum CameraDirection { NorthEast, NorthWest, SouthEast, SouthWest}
	private CameraDirection cameraDirection = CameraDirection.NorthEast;
	
	private float cameraRotationY = 225;

	// Use this for initialization
	void Start () {
        panSpeed = -panSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		HandleCameraMove ();		
		HandleCameraZoom ();
	}
 
	void HandleCameraZoom ()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
		
		if (camera != null) {
			if (camera.isOrthoGraphic) {
				camera.orthographicSize += scroll;
			} else {
				camera.fieldOfView += scroll;
			}
		}
	}


	void HandleCameraMove ()
	{
		Vector3 vector = new Vector3(Input.GetAxis("Horizontal") * panSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * panSpeed * Time.deltaTime);
		vector = Quaternion.AngleAxis(cameraAngle, Vector3.up) * vector;
		cameraTransform.Translate(vector, Space.World);
		
		if (Input.GetKeyUp("space")) {
			TurnCameraClockwise(); 
			cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(new Vector3(35f, cameraRotationY, 0f)), 1f);
		}
	}


	private void TurnCameraClockwise() {
		switch(cameraDirection){
			case CameraDirection.NorthEast:
				cameraTransform.Translate(new Vector3(0f, 0f, -30f), Space.World);
				cameraRotationY = 315f;
				cameraDirection = CameraDirection.SouthEast;
				break;
			case CameraDirection.NorthWest:
				cameraTransform.Translate(new Vector3(30f, 0f, 0f), Space.World);
				cameraRotationY = 225f;
				cameraDirection = CameraDirection.NorthEast;
				break;
			case CameraDirection.SouthEast:
				cameraTransform.Translate(new Vector3(-30f, 0f, 0f), Space.World);
				cameraRotationY = 45f;
				cameraDirection = CameraDirection.SouthWest;
				break;
			case CameraDirection.SouthWest:
				cameraTransform.Translate(new Vector3(0f, 0f, 30f), Space.World);
				cameraRotationY = 135f;
				cameraDirection = CameraDirection.NorthWest;
				break;
		}
	}
}