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

		switch(TargetWorld) {
			
			case MainMenuManager.Worlds.None:
				MainMenuReturnToLevelSelectBehaviour.selectedWorld = MainMenuReturnToLevelSelectBehaviour.SelectedWorld.None;
				break;
			case MainMenuManager.Worlds.Scotland:
				MainMenuReturnToLevelSelectBehaviour.selectedWorld = MainMenuReturnToLevelSelectBehaviour.SelectedWorld.Scotland;
				break;
			case MainMenuManager.Worlds.Netherlands:
				MainMenuReturnToLevelSelectBehaviour.selectedWorld = MainMenuReturnToLevelSelectBehaviour.SelectedWorld.Holland;
				break;
			case MainMenuManager.Worlds.Canada:
				MainMenuReturnToLevelSelectBehaviour.selectedWorld = MainMenuReturnToLevelSelectBehaviour.SelectedWorld.Canada;
				break;
			case MainMenuManager.Worlds.Australia:
				MainMenuReturnToLevelSelectBehaviour.selectedWorld = MainMenuReturnToLevelSelectBehaviour.SelectedWorld.Australia;
				break;

			//MainMenuReturnToLevelSelectBehaviour.selectedWorld = MainMenuReturnToLevelSelectBehaviour.SelectedWorld
		}
	}
}
