using UnityEngine;

/// <summary>
/// This script makes sure the 
/// </summary>
public class FoxMoveBehaviour : MonoBehaviour {
    private MoveBehaviour moveScript;

	// Use this for initialization
	void Start () {
	    moveScript = this.GetComponent<MoveBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (moveScript.enabled) {
	        return;
	    }

        moveScript.MoveInCurrentDirection();
	}
}
