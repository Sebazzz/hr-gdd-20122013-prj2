using UnityEngine;
using System.Collections;

public class EnemyMoveBehaviour : MonoBehaviour {

	public AudioSource audio;
	public AudioClip clip;
	private bool active = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!active) return;
		transform.position = Vector3.Lerp(transform.position, new Vector3(180f, transform.position.y, transform.position.z), 0.005f);

	}

	public void Activate() {
		active = true;
		audio.Stop();
		audio.clip = clip;
		audio.Play();
		audio.loop = true;
		
	}
}
