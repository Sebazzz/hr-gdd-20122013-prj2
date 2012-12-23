using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	public GameObject camera;
	public enum MenuIndex { TitleScreen, LevelSelect, Credits}
	public MenuIndex menuIndex = MenuIndex.TitleScreen;
	private float cameraRotation = 180f;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Update () {

		camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, Quaternion.Euler(0f, cameraRotation, 0f), Time.deltaTime*5f); 
	
	}

	/// <summary>
	/// Set Menu Index
	/// </summary>
	/// <param name="index">MenuIndex Enum</param>
	public void SetMenuIndex(MenuIndex index) {
		switch (index) {
			case MenuIndex.TitleScreen:
				cameraRotation = 180f;
				break;
			case MenuIndex.LevelSelect:
				cameraRotation = 270f;
				break;
			case MenuIndex.Credits:
				cameraRotation = 45f;
				break;
			default:
				cameraRotation = 180f;
				break;
		}
	}
}
