using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlHerderBehaviour : MonoBehaviour {
	public float waypointSpacing = 5f;
    
    private int layerMask = 1 << 8; // Layer 8 is de layer waar hij mee raycast.
    private float FAILSAFE = 50f; // Failsafe die er voor zorgt dat je geen rare paden krijgt. Stelt de Maximale afstand voor.

	public GameObject ShepherdPath;
	
    private bool listening = false;
    private Vector3 lastAdded = Vector3.zero;
    private Queue<Vector3> trajectory;
	private GameObject path;
	
	private LineRenderer line;
	
	void Start(){
		path = (GameObject)GameObject.Instantiate(ShepherdPath);
		line = path.GetComponent<LineRenderer>();
	}

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
			drawPath();
            GetComponent<HerderLoopBehaviour>().setTrajectory(trajectory);
        }

        if (listening) {
            Vector3 position = getPosition();
            if (trajectory.Count == 0) {
                trajectory.Enqueue(position);
                lastAdded = position;
            }

            if (Vector3.Distance(position, lastAdded) > waypointSpacing && Vector3.Distance(position, lastAdded) < FAILSAFE) {
                trajectory.Enqueue(position);
            }
       
        }

	}

    void OnDestroy() {
        if (this.line == null) {
            return;
        }

        this.line.SetVertexCount(0);
        this.line = null;
    }

    void OnGUI() {
        /*Vector2 pointA = Camera.main.WorldToViewportPoint(this.transform.position);
        Vector2 pointB = Camera.main.WorldToViewportPoint(Vector3.zero);

        Drawing.DrawLine(pointA, pointB);*/
    }

	private void drawPath(){
		Vector3[] t = trajectory.ToArray();
		line.SetVertexCount(t.Length);
		for(int i = 0; i < t.Length; i++) {
			Vector3 p = t[i];
			p.y += 0.2f;
        	line.SetPosition(i, p);
    	}
	}
	
	public void done(){
		line.SetVertexCount(0);
	}
	
    private Vector3 getPosition() {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {

            return hit.point;

        }

        return Vector3.zero;
    }
}
