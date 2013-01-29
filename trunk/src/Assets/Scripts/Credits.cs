using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour {

	public GameObject credits;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		credits.transform.Translate(Vector3.up * 0.05f);

		if (credits.transform.position.y > 160f) Application.LoadLevelAsync("MainMenu");
	
	}
}
