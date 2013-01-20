using UnityEngine;
using System.Collections;

public class MainMenuItemBehaviour : MonoBehaviour {

	public MainMenuManager.MenuIndex TargetIndex;
	public MainMenuManager.Worlds TargetWorld;
	public GameObject SceneManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseUpAsButton() {
		MainMenuManager manager;
		manager = SceneManager.GetComponent("MainMenuManager") as MainMenuManager;
		manager.SetMenuIndex(TargetIndex);
		manager.SetSelectedWorld(TargetWorld);
	}
}
