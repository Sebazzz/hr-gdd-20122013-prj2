using UnityEngine;
using System.Collections;

public class SheepIdleBehaviour : MonoBehaviour {

	public enum SheepState { inactive, active};
	public SheepState sheepState = SheepState.inactive;

    public float TimeInSecondsThreshold = 0.800f;

    private float timeCounter = 0;

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
		
        //if (sheepState == SheepState.inactive) {
        //    transform.LookAt(Camera.mainCamera.GetComponent<FollowFlockCameraBehaviour>().GetCameraLookAtTarget());
        //    Vector3 positionInSight = Camera.mainCamera.WorldToViewportPoint(transform.position);
        //    if (positionInSight.x >= 0.1f && positionInSight.x <= 0.9f &&
        //        positionInSight.y >= 0.1f && positionInSight.y <= 0.9f) {
        //        sheepState = SheepState.active;
        //        rigidbody.isKinematic = false;
        //        rigidbody.AddForce(transform.forward*15f, ForceMode.Impulse);
        //    }

        //    return; 
        //}

        if (sheepState == SheepState.inactive) {
            if (IsSeenInCamera()) {
                timeCounter += Time.deltaTime;

                if (timeCounter > TimeInSecondsThreshold) {
                    sheepState = SheepState.active;
                    rigidbody.isKinematic = false;
                    rigidbody.AddForce(transform.forward * 15f, ForceMode.Impulse);
                }
            } else {
                timeCounter = 0;
            }

            return;
        }
	}


    // http://answers.unity3d.com/questions/8003/how-can-i-know-if-a-gameobject-is-seen-by-a-partic.html
    private bool IsSeenInCamera() {
        Vector3 res = Camera.mainCamera.WorldToViewportPoint(this.transform.position);

        return res.x > 0.1f && res.x < 0.9f &&
               res.y > 0.1f && res.y < 0.9f &&
               res.z > 0;
    }
}
