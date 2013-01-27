using UnityEngine;
using System.Collections;

public class SheepEyeBehaviour : MonoBehaviour {

	public EnemyMoveBehaviour behaviour;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton() {
		behaviour.Activate();
	}
}
