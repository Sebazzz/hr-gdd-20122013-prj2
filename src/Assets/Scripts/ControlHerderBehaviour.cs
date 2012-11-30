using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlHerderBehaviour : MonoBehaviour {
    private bool listening = false;
    private Vector3 lastAdded = Vector3.zero;
    private Queue<Vector3> trajectory;

    public Queue<Vector3> getTrajectory() {
        return trajectory;
    }

    void OnMouseDown() {
        if (listening == false) { // Ingedrukt
            listening = true;
            trajectory = new Queue<Vector3>();
        }
    }

	void Update () {
        if (listening && Input.GetMouseButton(0) == false) { // Ingedrukt, nu losgelaten
            listening = false;
            GetComponent<HerderLoopBehaviour>().setTrajectory(trajectory);
        }

        if (listening) {
            Vector3 position = getPosition();
            if (trajectory.Count == 0) {
                trajectory.Enqueue(position);
                lastAdded = position;
            }

            if (Vector3.Distance(position, lastAdded) > 5) {
                trajectory.Enqueue(position);
            }
       
        }

	}

    private Vector3 getPosition() {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit)) {

            Debug.DrawLine (ray.origin, hit.point);
            return hit.point;

        }

        return Vector3.zero;
    }
}
