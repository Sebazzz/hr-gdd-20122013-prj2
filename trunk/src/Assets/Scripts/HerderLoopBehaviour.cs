using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HerderLoopBehaviour : MonoBehaviour {
    public float speed = 0.4f;
	public float acceptRadius = 5f; // Radius used for waypoints.

    private Queue<Vector3> trajectory;
    private Vector3 target = Vector3.zero;
    private bool walking = false;

	public void setTrajectory (Queue<Vector3> t) {
        trajectory = t;
        target = trajectory.Dequeue();
        walking = true;
	}

	void Update () {
        if (trajectory != null && trajectory.Count > 0 && reachedTarget()) { // Got a route!
            target = trajectory.Dequeue();
        }

        if (walking && trajectory.Count == 0 && reachedTarget()) {
            walking = false;
			GetComponent<ControlHerderBehaviour>().done();
        }
	}

    void FixedUpdate() {
        if (walking) {
            var lookRotation = Quaternion.LookRotation(target - transform.position, Vector3.forward);
    		lookRotation.x = 0;
   			lookRotation.z = 0;
    		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20);
            transform.Translate(0, 0, speed);
        }
    }

    bool reachedTarget() {
        if (Vector3.Distance(transform.position, target) < acceptRadius) {
            return true;
        }

        return false;
    }
}
