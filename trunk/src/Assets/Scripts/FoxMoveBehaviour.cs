using UnityEngine;
using System.Collections;

/// <summary>
/// This script makes sure the 
/// </summary>
public class FoxMoveBehaviour : MonoBehaviour {
    private MoveBehaviour moveScript;

    public int DeltaX = 5;
    public int DeltaY = 5;
    public int DeltaZ = 5;

	// Use this for initialization
	void Start () {
	    moveScript = this.GetComponent<MoveBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (moveScript.enabled) {
	        return;
	    }

        // find an position
	    Vector3 newPos = this.transform.position;
        newPos.x += DeltaX;
        newPos.y += DeltaY;
        newPos.z += DeltaZ;
	    
        // instruct the movement script
        moveScript.MoveTo(newPos);
	}
}
