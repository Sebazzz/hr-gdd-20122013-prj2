using UnityEngine;
using System.Collections;

public class SheepIdleBehaviour : MonoBehaviour {
    private SheepAudioController audioController;

    public float MinBlaatFrequency = 10; // In seconds
    public float MaxBlaatFrequency = 25;

	public enum SheepState { inactive, active};
	public SheepState sheepState = SheepState.inactive;
	public float minDistanceToScreenEdgeInViewPortCoordinates = 0.1f;
	public float JumpForce = 15f;

	private float minDistance;
	private float maxDistance;

    private Timer blaatTimer;

    void Awake() {
        this.audioController = this.GetComponent<SheepAudioController>();
    }

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
		minDistance = minDistanceToScreenEdgeInViewPortCoordinates;
		maxDistance = 1f - minDistanceToScreenEdgeInViewPortCoordinates;

		if(sheepState == SheepState.inactive)rigidbody.isKinematic = true;

        blaatTimer = new Timer(Random.Range(MinBlaatFrequency, MaxBlaatFrequency));
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () {
        Vector3 positionInSight = Camera.mainCamera.WorldToViewportPoint(transform.position);

        if (sheepState == SheepState.active && ((positionInSight.x >= minDistance && positionInSight.x <= maxDistance && positionInSight.y >= minDistance && positionInSight.y <= maxDistance))) {
            blaatTimer.Update();
            if (blaatTimer.IsTriggered) {
                audioController.IdleSound.Play();
                blaatTimer = new Timer(Random.Range(MinBlaatFrequency, MaxBlaatFrequency));
            }
        }
		
		if (sheepState == SheepState.inactive) {
			transform.LookAt(new Vector3(Camera.mainCamera.transform.position.x, transform.position.y, Camera.mainCamera.transform.position.z));
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

    private bool IsInCameraView() {
        Vector3 positionInSight = Camera.mainCamera.WorldToViewportPoint(transform.position);

        return positionInSight.x >= minDistance && positionInSight.x <= maxDistance &&
               positionInSight.y >= minDistance && positionInSight.y <= maxDistance;
    }
}
