using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed class MainMenuDeveloperWipeSettingsBehaviour : MonoBehaviour {
    private void OnMouseUpAsButton() {
        if (Debug.isDebugBuild && Input.GetKey(KeyCode.RightShift) &&  Input.GetKey(KeyCode.LeftControl)) {
            PlayerPrefs.DeleteAll();

            AsyncSceneLoader.Load(Scenes.MainMenu);
        }

    }

}