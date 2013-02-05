using UnityEngine;

public sealed class MainMenuLoadPlaygroundBehaviour : MonoBehaviour {
    private int count = 0;

	public string scenetoload = Scenes.Playground;
	
	void OnMouseUpAsButton() {
		AsyncSceneLoader.Load(scenetoload);
    }
}