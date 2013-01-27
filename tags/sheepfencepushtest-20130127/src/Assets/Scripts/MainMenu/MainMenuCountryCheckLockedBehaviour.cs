using UnityEngine;
using System.Collections;

public class MainMenuCountryCheckLockedBehaviour : MonoBehaviour {

	public int countryIndex;
	public Material levelUnlocked;
	public Material levelLocked;
	public MeshRenderer planeMesh;
	public Collider collider;

	// Use this for initialization
	void Start () {
		
		if (countryIndex != 0) {
			if (Levels.Countries[countryIndex - 1].HasBeenCompleted()) {
				planeMesh.material = levelUnlocked;
				collider.enabled = true;

				if (Levels.GetLevelByName(Levels.Countries[countryIndex].Levels[0].Name).GetState() == Level.LevelStatus.Locked)
					Levels.GetLevelByName(Levels.Countries[countryIndex].Levels[0].Name).Unlock();
			}
			else {
				planeMesh.material = levelLocked;
				collider.enabled = false;
			}
		}
		else {
			planeMesh.material = levelUnlocked;
			collider.enabled = true;
		}

		//Debug.Log(Levels.Countries[0].HasBeenCompleted());
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}
}
