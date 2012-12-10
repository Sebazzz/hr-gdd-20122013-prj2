using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HerderLoopBehaviour : MonoBehaviour {
    public float MIN_SPEED = 0.5f; // Minimal speed.
    public float SPEED_FACTOR = 2f; // MAX_DRAWTIME - Tijd wordt vermenigvuldigd met dit getal om zo een snelheid te creeeren.
    public static float MAX_DRAWTIME = 0.5f; // Tijd in seconden voor 1 segment van de lijn.
	public float acceptRadius = 5f; // Radius used for waypoints.

    private float speed = 0;
    private Queue<Vector3> trajectory;
    private Vector3 target = Vector3.zero;
    private bool walking = false;

	public void setTrajectory (Queue<Vector3> t, float drawTime) {
        trajectory = t;
        target = trajectory.Dequeue();
        walking = true;
        //if(drawTime > MAX_DRAWTIME) drawTime = MAX_DRAWTIME;
        speed = MIN_SPEED + ( (MAX_DRAWTIME - drawTime) * SPEED_FACTOR );
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
            var lookPos = target - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * 20);
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
