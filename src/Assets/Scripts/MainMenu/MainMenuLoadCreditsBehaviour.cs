using UnityEngine;
using System.Collections;

public class MainMenuLoadCreditsBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton() {
		AsyncSceneLoader.Load("Credits");
	}
}
