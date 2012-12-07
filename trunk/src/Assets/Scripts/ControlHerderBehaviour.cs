using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Bestuurt de herdershond. Heeft een getTrajectory functie die je aan kan roepen, om het huidige getekende path te krijgen.
/// </summary>
public class ControlHerderBehaviour : MonoBehaviour {
	public float waypointSpacing = 5f;
    public Material slow_material;
    public Material normal_material;
    public Material fast_material;

    
    
    private int layerMask = 1 << 8; // Layer 8 is de layer waar hij mee raycast.
    private float FAILSAFE = 50f; // Failsafe die er voor zorgt dat je geen rare paden krijgt. Stelt de Maximale afstand voor.

	public GameObject ShepherdPath;
	
    private bool listening = false;
    private Vector3 lastAdded = Vector3.zero;
    private Queue<Vector3> trajectory;
	private GameObject path;
	
	private LineRenderer line;

    private float startTime, endTime = 0; // Tijd in seconden.
	
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
            startTime = Time.time;
        }
    }

	void Update () {
        if (listening && Input.GetMouseButton(0) == false) { // Ingedrukt, nu losgelaten
            endTime = Time.time;
            listening = false;
			drawPath();
            float drawTime = endTime - startTime;
            float speedSegment = HerderLoopBehaviour.MAX_DRAWTIME / 3;
            if (drawTime < speedSegment){
                line.renderer.material = slow_material;
            }else if(drawTime < speedSegment*2){
                line.renderer.material = normal_material;
            }else{
                line.renderer.material = fast_material;
            }
            GetComponent<HerderLoopBehaviour>().setTrajectory(trajectory, drawTime);
        }

        if (listening) {
            Vector3 position = getPosition();
            if (trajectory.Count == 0) {
                trajectory.Enqueue(position);
                lastAdded = position;
            }
            
            if (Vector3.Distance(position, lastAdded) > waypointSpacing && Vector3.Distance(position, lastAdded) < FAILSAFE) {
                trajectory.Enqueue(position);
                lastAdded = position;
            }
       
        }

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
