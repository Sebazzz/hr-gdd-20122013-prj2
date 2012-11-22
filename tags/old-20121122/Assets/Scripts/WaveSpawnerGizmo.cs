using UnityEngine;
using System.Collections;

public class WaveSpawnerGizmo : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "BloodSpatter.png", true);
    }
}
