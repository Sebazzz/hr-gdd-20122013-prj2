using UnityEngine;
using System.Collections;

public class MainMenuLoadLevelBehaviour : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton() {
		Application.LoadLevel(Levels.getLevelByName("Playground").name);
	}
}
