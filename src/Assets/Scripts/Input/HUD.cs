using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        if (GUI.Button(new Rect(10, 70, 100, 30), "Restart Scene")) {
            Application.LoadLevel(Application.loadedLevel);
        }

        if (GUI.Button(new Rect(10, 110, 100, 30), "Scene select")) {
            Application.LoadLevel(Scenes.MainMenu);
        }
    }
}
