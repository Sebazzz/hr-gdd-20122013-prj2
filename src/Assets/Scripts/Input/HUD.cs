using UnityEngine;
using System.Collections;
using System;

public class HUD : MonoBehaviour {

    private bool showDialog = false; // Is set to true once a dialog is shown
    private Rect dialogRect;

    //Dialog data
    private string dialog_title = "";
    private string dialog_text = "";
    private string dialog_yes = "Yes";
    private string dialog_no = "No";
    private Action action_yes;
    private Action action_no;

	// Use this for initialization
	void Start () {
        dialogRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        drawTopBar();

        if (showDialog) {
            drawDialog();
        }
        
    }

    /// <summary>
    /// Displays A YES-NO dialog. See: http://stackoverflow.com/questions/667742/callbacks-in-c-sharp
    /// </summary>
    /// <param name="title">Title for the dialog</param>
    /// <param name="text">Text to display in dialog</param>
    /// <param name="yes_action">Action delegate to run for Yes</param>
    /// <param name="no_action">Action delegate to run for No</param>
    public void DisplayDialog(string title, string text, Action yes_action, Action no_action) {
        dialog_title = title;
        dialog_text = text;
        action_yes = yes_action;
        action_no = no_action;
        showDialog = true;
    }

    private void drawDialog() {
        dialogRect = GUI.Window(0, dialogRect, drawInsideDialog, dialog_title);
    }

    private void drawInsideDialog(int dialogID) {
        GUI.Label(new Rect(25, 25, 250, 130), dialog_text);

        if (GUI.Button(new Rect(30, 160, 50, 30), dialog_yes)) {
            action_yes();
            showDialog = false;
        }

        if (GUI.Button(new Rect(220, 160, 50, 30), dialog_no)) {
            action_no();
            showDialog = false;
        }
    }

    private void drawTopBar() {
        GUI.Label(new Rect(25, 15, 100, 30), "1/5 Schapen");

        GUI.Label(new Rect(125, 15, 100, 30), "00:12");

        if (GUI.Button(new Rect(10, 70, 100, 30), "Restart")) {
            Application.LoadLevel(Application.loadedLevel);
        }

        if (GUI.Button(new Rect(10, 110, 100, 30), "Level select")) {
            Application.LoadLevel(Scenes.MainMenu);
        }
    }
}
