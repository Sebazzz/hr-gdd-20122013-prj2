using UnityEngine;
using System.Collections;

public class CheckAudioOptionBehaviour : MonoBehaviour {

	public AudioListener listener;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		switch (PlayerPrefs.GetInt("audio", 0)) {
			case 0:
				listener.enabled = true;
				break;
			case 1:
				listener.enabled = false;
				break;
		}
	}
}
