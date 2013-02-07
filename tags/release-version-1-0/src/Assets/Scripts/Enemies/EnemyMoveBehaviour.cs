using UnityEngine;
using System.Collections;

public class EnemyMoveBehaviour : MonoBehaviour {

	public AudioSource AudioToChange;
	public AudioClip newAudioClip;
	private bool enemyIsActive = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!enemyIsActive) return;
		transform.position = Vector3.Lerp(transform.position, new Vector3(180f, transform.position.y, transform.position.z), 0.005f);

	}

	public void ActivateEnemy() {
		enemyIsActive = true;
		AudioToChange.Stop();
		AudioToChange.clip = newAudioClip;
		AudioToChange.Play();
		AudioToChange.loop = true;
		
	}
}
