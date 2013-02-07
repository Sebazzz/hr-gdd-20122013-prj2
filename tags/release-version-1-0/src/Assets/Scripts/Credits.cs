using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour {

	public GameObject credits;

    public float ScrollSpeed = 10f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        credits.transform.Translate(Vector3.up * ScrollSpeed * Time.deltaTime);

		if (credits.transform.position.y > 160f) Application.LoadLevelAsync("MainMenu");
	
	}
}
