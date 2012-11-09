using UnityEngine;
using System.Collections;

public class RTSCameraBehaviour : MonoBehaviour {
    public Transform cameraTransform;
    public float scrollSpeed = 10;

	// Use this for initialization
	void Start () {
        scrollSpeed = -scrollSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 vector = new Vector3(Input.GetAxis("Horizontal") * scrollSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * scrollSpeed * Time.deltaTime);
        vector = Quaternion.AngleAxis(45, Vector3.up) * vector;
        cameraTransform.Translate(vector, Space.World);
	}
}
