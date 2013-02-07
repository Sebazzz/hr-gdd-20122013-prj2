using UnityEngine;
using System.Collections;

public class MainMenuCountryCheckLockedBehaviour : MonoBehaviour {

	public int countryIndex;
	public Material levelUnlocked;
	public Material levelLocked;
	public MeshRenderer planeMesh;
	public Collider meshCollider;

	// Use this for initialization
	void Start () {
		
		if (countryIndex != 0) {
			if (Levels.Countries[countryIndex - 1].HasBeenCompleted()) {
				planeMesh.material = levelUnlocked;
				meshCollider.enabled = true;

				if (Levels.GetLevelByName(Levels.Countries[countryIndex].Levels[0].Name).GetState() == Level.LevelStatus.Locked)
					Levels.GetLevelByName(Levels.Countries[countryIndex].Levels[0].Name).Unlock();
			}
			else {
				planeMesh.material = levelLocked;
				meshCollider.enabled = false;
			}
		}
		else {
			planeMesh.material = levelUnlocked;
			meshCollider.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}
}
