using UnityEngine;
using System.Collections;

public class RTSCameraBehaviour : MonoBehaviour {
    public Transform cameraTransform;
    public float panSpeed = 20;
	public float zoomSpeed = 25;
	public float cameraAngle = 45;

	// Use this for initialization
	void Start () {
        panSpeed = -panSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		HandleCameraMove ();
		
		HandleCharacterSelection ();
		
		HandleCameraZoom ();
	}

	void HandleCameraZoom ()
	{
		float scroll = Input.GetAxis("ZoomWheel") * zoomSpeed * Time.deltaTime;
		
		if (camera != null) {
			if (camera.isOrthoGraphic) {
				camera.orthographicSize += scroll;
			} else {
				camera.fieldOfView += scroll;
			}
		}
	}

	void HandleCharacterSelection ()
	{
		if(Input.GetMouseButtonDown(1)){
			ShootRay();
		}
	}

	void HandleCameraMove ()
	{
		Vector3 vector = new Vector3(Input.GetAxis("Horizontal") * panSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * panSpeed * Time.deltaTime);
		vector = Quaternion.AngleAxis(cameraAngle, Vector3.up) * vector;
		cameraTransform.Translate(vector, Space.World);
	}
	
	void ShootRay(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

 		RaycastHit hit;
		
    	if(Physics.Raycast(ray, out hit)){
			Vector3 point = hit.point;
			// make sure objects dont move on the Y axis
			point.y = 1;
			
			ActionWalk aw = new ActionWalk(point);
        	ObjectManager.getInstance().action(aw);
			if(hit.collider.gameObject.tag == "enterable"){
				ActionBuilding ab = new ActionBuilding(hit.collider.gameObject);
        		ObjectManager.getInstance().action(ab);
			}
		}

	}
}
