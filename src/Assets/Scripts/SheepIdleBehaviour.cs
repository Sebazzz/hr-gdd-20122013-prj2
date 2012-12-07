using UnityEngine;
using System.Collections;

public class SheepIdleBehaviour : MonoBehaviour {

	public enum SheepState { inactive, active};
	public SheepState sheepState = SheepState.inactive;
	public Rigidbody rigidbody;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
		if(sheepState == SheepState.inactive)rigidbody.isKinematic = true;
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () {
		
		if (sheepState == SheepState.inactive) {
			transform.LookAt(Camera.mainCamera.GetComponent<FollowFlockCameraBehaviour>().GetCameraLookAtTarget());
			Vector3 positionInSight = Camera.mainCamera.WorldToViewportPoint(transform.position);
			if (positionInSight.x >= 0.1f && positionInSight.x <= 0.9f &&
				positionInSight.y >= 0.1f && positionInSight.y <= 0.9f) {
				sheepState = SheepState.active;
				rigidbody.isKinematic = false;
				rigidbody.AddForce(transform.forward*15f, ForceMode.Impulse);
			}

			return; 
		}

	}
}
