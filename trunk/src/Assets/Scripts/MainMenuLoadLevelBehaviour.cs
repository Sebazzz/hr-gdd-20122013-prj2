using UnityEngine;
using System.Collections;

public class MainMenuLoadLevelBehaviour : MonoBehaviour {
    public string LevelName = "Playground";
	public Material levelUnlocked;
	public Material levelLocked;
	public MeshRenderer planeMesh;

	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll();

		//Levels.GetLevelByName("Scotland_lvl1").SetFinished();
		//Levels.GetLevelByName("Scotland_lvl2").SetFinished();
		//Levels.GetLevelByName("Scotland_lvl3").SetFinished();
		//Levels.GetLevelByName("Scotland_lvl4").SetFinished();

		//Levels.GetLevelByName("Holland_lvl1").SetFinished();
		//Levels.GetLevelByName("Holland_lvl2").SetFinished();
		//Levels.GetLevelByName("Holland_lvl3").SetFinished();
		//Levels.GetLevelByName("Holland_lvl4").SetFinished();

		//Levels.GetLevelByName("Australia_lvl1").SetFinished();
		//Levels.GetLevelByName("Australia_lvl2").SetFinished();
		//Levels.GetLevelByName("Australia_lvl3").SetFinished();
		//Levels.GetLevelByName("Australia_lvl4").SetFinished();

		//Levels.GetLevelByName("Level1").SetFinished();
		//Levels.GetLevelByName("Level2").SetFinished();
		//Levels.GetLevelByName("Level3").SetFinished();
		//Levels.GetLevelByName("Level4").SetFinished();

		if (LevelName == "Scotland_lvl1" && Levels.GetLevelByName(this.LevelName).GetState() == Level.LevelStatus.Locked) {
			Levels.GetLevelByName(this.LevelName).Unlock();
		}

		if (Levels.GetLevelByName(this.LevelName).GetState() == Level.LevelStatus.Locked) planeMesh.material = levelLocked;
		else planeMesh.material = levelUnlocked;
	}
	
	// Update is called once per frame
	void Update () {
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
