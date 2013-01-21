using UnityEngine;

public sealed class MainMenuLoadPlaygroundBehaviour : MonoBehaviour {
    private int count = 0;
    void OnMouseUpAsButton() {
        if (++count >= 3) {
            AsyncSceneLoader.Load(Scenes.Playground);
        }
    }
}