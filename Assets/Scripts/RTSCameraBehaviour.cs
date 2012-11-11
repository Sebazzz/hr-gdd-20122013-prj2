using UnityEngine;
using System.Collections;

public class RTSCameraBehaviour : MonoBehaviour {
    public Transform cameraTransform;
    public float scrollSpeed = 20;

	// Use this for initialization
	void Start () {
        scrollSpeed = -scrollSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 vector = new Vector3(Input.GetAxis("Horizontal") * scrollSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * scrollSpeed * Time.deltaTime);
        vector = Quaternion.AngleAxis(45, Vector3.up) * vector;
        cameraTransform.Translate(vector, Space.World);
		
		if(Input.GetMouseButtonDown(1)){
			shootRay();
		}
	}
	
	void shootRay(){
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
