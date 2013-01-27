using UnityEngine;
using System.Collections;

public class MainMenuLoadLevelBehaviour : MonoBehaviour {
	public string LevelName = "Playground";
	public Material levelUnlocked;
	public Material levelLocked;
	public MeshRenderer planeMesh;
	public MeshRenderer minSheep;
	public MeshRenderer maxSheep;
	public MeshRenderer time;

	// Use this for initialization
	void Start() {

		//PlayerPrefs.DeleteAll();

		//Levels.GetLevelByName("Scotland_lvl1").SetFinished();
		//Levels.GetLevelByName("Scotland_lvl2").SetFinished();
		//Levels.GetLevelByName("Scotland_lvl3").SetFinished();
		//Levels.GetLevelByName("Scotland_lvl4").SetFinished();

		//Levels.GetLevelByName("Holland_lvl1").SetFinished();
		//Levels.GetLevelByName("Holland_lvl2").SetFinished();
		//Levels.GetLevelByName("Holland_lvl3").SetFinished();
		//Levels.GetLevelByName("Holland_lvl4").SetFinished();

		minSheep.renderer.enabled = false;
		maxSheep.renderer.enabled = false;
		time.renderer.enabled = false;

		if (LevelName == "Scotland_lvl1" && Levels.GetLevelByName(this.LevelName).GetState() == Level.LevelStatus.Locked) {
			Levels.GetLevelByName(this.LevelName).Unlock();
		}

		if (Levels.GetLevelByName(this.LevelName).GetState() == Level.LevelStatus.Locked) planeMesh.material = levelLocked;
		else {
			planeMesh.material = levelUnlocked;

			if (Levels.GetLevelByName(this.LevelName).HasFlag(Level.Score.MinSheep)) minSheep.renderer.enabled = true;

			if (Levels.GetLevelByName(this.LevelName).HasFlag(Level.Score.MaxSheep)) maxSheep.renderer.enabled = true;

			if (Levels.GetLevelByName(this.LevelName).HasFlag(Level.Score.MaxSheepWithinTime)) time.renderer.enabled = true;
		}
	}

	// Update is called once per frame
	void Update() {
	}

	void OnMouseUpAsButton() {
		if (Debug.isDebugBuild && Input.GetKey(KeyCode.RightShift)) {
			Levels.GetLevelByName(this.LevelName).Unlock();
		}

		//Debug.Log(Levels.GetLevelByName(this.LevelName).GetState());

		if (Levels.GetLevelByName(this.LevelName).GetState() == Level.LevelStatus.Locked) return;

		AsyncSceneLoader.Load(Levels.GetLevelByName(this.LevelName).Name);
	}
}