using UnityEngine;
using System.Collections;

public class SheepIdleBehaviour : MonoBehaviour {
    public AudioClip SOUND_BLAAT; // Het blaatgeluid.
    public float MinBlaatFrequency = 10; // In seconds
    public float MaxBlaatFrequency = 25;

	public enum SheepState { inactive, active};
	public SheepState sheepState = SheepState.inactive;
	public float minDistanceToScreenEdgeInViewPortCoordinates = 0.1f;
	public float JumpForce = 15f;

	private float minDistance;
	private float maxDistance;

    private float lastBlaatTime = 0;
    private float blaatTime = 0;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
		minDistance = minDistanceToScreenEdgeInViewPortCoordinates;
		maxDistance = 1f - minDistanceToScreenEdgeInViewPortCoordinates;

		if(sheepState == SheepState.inactive)rigidbody.isKinematic = true;

        lastBlaatTime = Time.time;
        blaatTime = Random.Range(MinBlaatFrequency, MaxBlaatFrequency);
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () {

        if (sheepState == SheepState.active) {
            if (Time.time - lastBlaatTime >= blaatTime) {
                audio.PlayOneShot(SOUND_BLAAT);
                blaatTime = Random.Range(MinBlaatFrequency, MaxBlaatFrequency);
                lastBlaatTime = Time.time;
            }
        }
		
		if (sheepState == SheepState.inactive) {
			transform.LookAt(new Vector3(Camera.mainCamera.transform.position.x, transform.position.y, Camera.mainCamera.transform.position.z));
			Vector3 positionInSight = Camera.mainCamera.WorldToViewportPoint(transform.position);
			if (positionInSight.x >= minDistance && positionInSight.x <= maxDistance &&
				positionInSight.y >= minDistance && positionInSight.y <= maxDistance) {
				sheepState = SheepState.active;
				rigidbody.isKinematic = false;
				rigidbody.AddForce(transform.forward*JumpForce, ForceMode.Impulse);

			    //this.enabled = false;
			}

			return; 
		}

	}
}
