using UnityEngine;
using System.Collections;

public class MainMenuOptionsSaveBehaviour : MonoBehaviour {

	public MainMenuOptionsBehaviour optionsBehaviour;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton() {

		optionsBehaviour.SaveOptions();
		
	}
}
