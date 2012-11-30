using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HerderLoopBehaviour : MonoBehaviour {
    public float speed = 0.4f;

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
        }
	}

    void FixedUpdate() {
        if (walking) {
            transform.LookAt(target);
            transform.Translate(0, 0, speed);
        }
    }

    bool reachedTarget() {
        if (Vector3.Distance(transform.position, target) < 2) {
            return true;
        }

        return false;
    }
}
