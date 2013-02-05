using UnityEngine;
using System.Collections;

public class ShouldShowHTPBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (Levels.GetLevelByName("Scotland_lvl1").GetState() != Level.LevelStatus.Done) {
            showDialog();
        }
	}

    private void showDialog() {
        Camera.mainCamera.GetComponent<CameraZoomStartController>().enabled = false;
        HUD.Instance.Show = false;

        HowToPlayDialog.ShowDialog(new HowToPlayDialog.ButtonInfo("", delegate() {
            Camera.mainCamera.GetComponent<CameraZoomStartController>().enabled = true;
            HUD.Instance.Show = true;
        }));
    }

}
