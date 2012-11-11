using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject friend = GameObject.FindWithTag("friend");
        transform.LookAt(friend.transform);
		transform.Translate(0, 0, 1f * Time.deltaTime);
	}
}
