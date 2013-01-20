using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	public enum MenuIndex { TitleScreen, WorldSelect, LevelSelect, Options}
	public enum Worlds { None, Scotland, Netherlands, Canada, Australia}
	public MenuIndex menuIndex = MenuIndex.TitleScreen;
	public Worlds selectedWorld = Worlds.None;
	
	public GameObject LevelSelectScotland;
	public GameObject LevelSelectNetherlands;
	public GameObject LevelSelectCanada;
	public GameObject LevelSelectAustralia;
	
	private float cameraRotation = 180f;
	private const float cameraHigh = 53.19862f;
	private const float cameraLow = 51.19862f;
	private float cameraYPosition = 51.19862f;
	
	public Camera camera;

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
		camera.transform.position = Vector3.Slerp(camera.transform.position, new Vector3(camera.transform.position.x, cameraYPosition, camera.transform.position.z), Time.deltaTime * 5f);
	}

	/// <summary>
	/// Set Menu Index
	/// </summary>
	/// <param name="index">MenuIndex Enum</param>
	public void SetMenuIndex(MenuIndex index) {
		switch (index) {
			case MenuIndex.TitleScreen:
				cameraRotation = 180f;
				cameraYPosition = cameraLow;
				break;
			case MenuIndex.WorldSelect:
				cameraRotation = 270f;
				cameraYPosition = cameraLow;
				break;
			case MenuIndex.LevelSelect:
				cameraRotation = 270f;
				cameraYPosition = cameraHigh;
				break;
			case MenuIndex.Options:
				cameraRotation = 45f;
				cameraYPosition = cameraLow;
				break;
			default:
				cameraRotation = 180f;
				cameraYPosition = cameraLow;
				break;
		}
	}

	public void SetSelectedWorld(Worlds index) {
		switch (index) {
			case Worlds.None:
				LevelSelectScotland.SetActive(false);
				LevelSelectNetherlands.SetActive(false);
				LevelSelectCanada.SetActive(false);
				LevelSelectAustralia.SetActive(false);
				break;
			case Worlds.Scotland:
				LevelSelectScotland.SetActive(true);
				LevelSelectNetherlands.SetActive(false);
				LevelSelectCanada.SetActive(false);
				LevelSelectAustralia.SetActive(false);
				break;
			case Worlds.Netherlands:
				LevelSelectScotland.SetActive(false);
				LevelSelectNetherlands.SetActive(true);
				LevelSelectCanada.SetActive(false);
				LevelSelectAustralia.SetActive(false);
				break;
			case Worlds.Canada:
				LevelSelectScotland.SetActive(false);
				LevelSelectNetherlands.SetActive(false);
				LevelSelectCanada.SetActive(true);
				LevelSelectAustralia.SetActive(false);
				break;
			case Worlds.Australia:
				LevelSelectScotland.SetActive(false);
				LevelSelectNetherlands.SetActive(false);
				LevelSelectCanada.SetActive(false);
				LevelSelectAustralia.SetActive(true);
				break;
			default:
				LevelSelectScotland.SetActive(false);
				LevelSelectNetherlands.SetActive(false);
				LevelSelectCanada.SetActive(false);
				LevelSelectAustralia.SetActive(false);
				break;
		}
	}
}
