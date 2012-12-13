using UnityEngine;
using System.Collections;

public class SheepIdleBehaviour : MonoBehaviour {

	public enum SheepState { inactive, active};
	public SheepState sheepState = SheepState.inactive;
	public float minDistanceToScreenEdgeInViewPortCoordinates = 0.1f;
	public float JumpForce = 15f;

	private float minDistance;
	private float maxDistance;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
		minDistance = minDistanceToScreenEdgeInViewPortCoordinates;
		maxDistance = 1f - minDistanceToScreenEdgeInViewPortCoordinates;

		if(sheepState == SheepState.inactive)rigidbody.isKinematic = true;
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () {
		
		if (sheepState == SheepState.inactive) {
			transform.LookAt(new Vector3(Camera.mainCamera.transform.position.x, transform.position.y, Camera.mainCamera.transform.position.z));
			Vector3 positionInSight = Camera.mainCamera.WorldToViewportPoint(transform.position);
			if (positionInSight.x >= minDistance && positionInSight.x <= maxDistance &&
				positionInSight.y >= minDistance && positionInSight.y <= maxDistance) {
				sheepState = SheepState.active;
				rigidbody.isKinematic = false;
				rigidbody.AddForce(transform.forward*JumpForce, ForceMode.Impulse);

			    this.enabled = false;
			}

			return; 
		}

	}
}
