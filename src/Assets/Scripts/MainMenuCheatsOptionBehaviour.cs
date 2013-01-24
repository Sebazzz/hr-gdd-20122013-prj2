using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;

public class MainMenuCheatsOptionBehaviour : MonoBehaviour {
    private Rect dialogRect;
    private bool showDialog;
    private string enteredCheat;

	// Use this for initialization
	void Start () {
	}

    // Update is called once per frame
	void Update () {
	
	}

    void OnMouseUpAsButton() {
        this.showDialog = true;
        this.enteredCheat = String.Empty;
        this.dialogRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200);
    }

    void OnGUI() {
        if (showDialog) {
            this.DrawDialog();
        }
    }

    private void DrawDialog() {
        dialogRect = GUI.Window(0, this.dialogRect, this.DrawInsideDialog, "Cheats");
    }

    private void DrawInsideDialog(int dialogId) {
        GUI.Label(new Rect(25, 25, 250, 25), "Enter cheat: ");

        this.enteredCheat = GUI.TextField(new Rect(25, 60, 250, 25), this.enteredCheat, 60);

        if (GUI.Button(new Rect(220, 160, 50, 30), "Apply")) {
            this.showDialog = false;

            if (String.IsNullOrEmpty(this.enteredCheat)) {
                this.enteredCheat = String.Empty;
            } else {
                ApplyCheat(this.enteredCheat);
            }
        }
    }

    private static void ApplyCheat(string cheatText) {
        // select correct cheat member
        MethodInfo cheatMember = null;
        MethodInfo[] cheatMembers = typeof(Cheats).GetMethods(BindingFlags.Public | BindingFlags.Static);

        foreach (MethodInfo member in cheatMembers) {
            // get cheat attr
            object[] attr = member.GetCustomAttributes(typeof (CheatAttribute), false);

            if (attr.Length == 0) {
                continue;
            }

            CheatAttribute cheatAttr = attr[0] as CheatAttribute;
            if (cheatAttr == null) {
                continue;
            }

            // compare
            if (String.Equals(cheatAttr.Name, cheatText, StringComparison.InvariantCultureIgnoreCase)) {
                cheatMember = member;
                break;
            }
        }

        if (cheatMember == null) {
            return;
        }

        // call
        cheatMember.Invoke(null, null);

        Debug.Log("Applied cheat: " + cheatMember.Name);
    }


    private static class Cheats {
        [Cheat("PlayTheGround")]
        public static void LoadPlaygroundCheat() {
            AsyncSceneLoader.Load(Scenes.Playground);
        }

        [Cheat("ClearSettings")]
        public static void ClearSettings() {
            PlayerPrefs.DeleteAll();

            AsyncSceneLoader.Load(Scenes.MainMenu);
        }

        [Cheat("BeTheBest")]
        public static void SetAllLevelsPlayed() {
            Level currentLevel = Levels.GetFirstLevel();

            while (currentLevel != Level.None) {
                currentLevel.SetFinished();

                currentLevel = Levels.GetNextLevel(currentLevel);
            }

            AsyncSceneLoader.Load(Scenes.MainMenu);
        }
    }

    private sealed class CheatAttribute : Attribute {
        private readonly string name;

        public string Name {
            get { return this.name; }
        }

        public CheatAttribute(string name) {
            this.name = name;
        }
    }
}
