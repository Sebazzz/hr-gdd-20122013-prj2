using UnityEngine;
using System.Collections;

/// <summary>
/// This script makes sure the 
/// </summary>
public class RandomMoveBehaviour : MonoBehaviour {

    private GameObject thePlane;
    private MoveBehaviour moveScript;

	// Use this for initialization
	void Start () {
	    thePlane = GameObject.Find("Plane");

	    moveScript = this.GetComponent<MoveBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (moveScript.enabled) {
	        return;
	    }

        // find an random position on the plane
	    Vector3 randomPos = GetRandomPosition(thePlane.collider.bounds);
	    
        // instruct the movement script
        moveScript.MoveTo(randomPos);
	}


    private Vector3 GetRandomPosition(Bounds bounds) {
        Vector3 pos = new Vector3();

        pos.x = Random.Range(bounds.min.x, bounds.max.x);
        pos.y = bounds.center.y;
        pos.z = Random.Range(bounds.min.z, bounds.max.z);

        return pos;
    }

}
