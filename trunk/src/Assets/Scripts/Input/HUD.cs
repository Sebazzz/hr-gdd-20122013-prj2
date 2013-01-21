using UnityEngine;
using System.Collections;
using System;

public class HUD : MonoBehaviour {

    // Skin data
    public GUISkin skin;

    private bool showDialog = false; // Is set to true once a dialog is shown
    private Rect dialogRect;

    //Dialog data
    private string dialog_title = "";
    private string dialog_text = "";
    private string dialog_yes = "Yes";
    private string dialog_no = "No";
    private Action action_yes;
    private Action action_no;

    private Texture2D timeTexture;
    private Texture2D goalTexture;
    private Texture2D sheepTexture;

    private int goal = 0;
    private int collected = 0;
    private int maxCollected = 0;

    public float LevelTime { get; set; }


	// Use this for initialization
	void Start () {
        dialogRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200);

        if (skin == null) {
            throw(new Exception("GUISkin is needed for the HUD"));
        }

        loadTextures();

	}

    private void loadTextures() {
        timeTexture = Resources.Load("Hud/hud-time") as Texture2D;
        goalTexture = Resources.Load("Hud/hud-goal") as Texture2D;
        sheepTexture = Resources.Load("Hud/hud-sheep") as Texture2D;
    }
	
	// Update is called once per frame
	void Update () {
	    this.LevelTime -= Time.deltaTime;
	}

    void OnGUI() {
        drawTopBar();

        if (showDialog) {
            drawDialog();
        }
        
    }

    /// <summary>
    /// Sets goal
    /// </summary>
    /// <param name="goal"></param>
    public void setGoal(int goal) {
        this.goal = goal;
    }

    /// <summary>
    /// Sets collected.
    /// </summary>
    /// <param name="amount">Is value 0 in "sheep 0/10"</param>
    public void setCollected(int amount) {
        collected = amount;
    }

    /// <summary>
    /// Sets maxCollected
    /// </summary>
    /// <param name="amount">Is value 10 in "sheep 0/10"</param>
    public void setMaxCollected(int amount) {
        maxCollected = amount;
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
        GUI.DrawTexture(new Rect(pixelsFromLeft(20), 20, 115, 59), timeTexture, ScaleMode.StretchToFill, true, 0);
        GUI.Label(new Rect(pixelsFromLeft(70), 35, 100, 40), this.GetTimeAsString(), this.LevelTime > 0 ? skin.label : skin.GetStyle("LabelRedTime"));

        GUI.DrawTexture(new Rect(pixelsFromLeft(155), 20, 145, 59), goalTexture, ScaleMode.StretchToFill, true, 0);
        GUI.Label(new Rect(pixelsFromLeft(246), 35, 10, 30), getGoal(), skin.GetStyle("LabelWhite"));

        GUI.DrawTexture(new Rect(pixelsFromLeft(320), 20, 230, 59), sheepTexture, ScaleMode.StretchToFill, true, 0);
        GUI.Label(new Rect(pixelsFromLeft(445), 35, 50, 40), getCollected(), skin.GetStyle("LabelRed"));// x = 465 maar omdat we rechts uitlijnen is het x - width
        GUI.Label(new Rect(pixelsFromLeft(495), 35, 100, 40), getMaxCollected(), skin.GetStyle("LabelBlack"));

        if (GUI.Button(new Rect(pixelsFromRight(190), 20, 55, 59), "", skin.GetStyle("RestartButton"))) {
            DisplayDialog("Restart", "Would you like to restart this level?",
                          () => Application.LoadLevel(Application.loadedLevel), delegate() { });
        }

        if (GUI.Button(new Rect(pixelsFromRight(115), 20, 95, 59), "", skin.GetStyle("MenuButton"))) {
            DisplayDialog("Return to menu", "Would you like to return to the menu?",
                          () => Application.LoadLevel(Scenes.MainMenu), () => { });
            
        }
    }

    private string GetTimeAsString(){
        if (this.LevelTime <= 0) {
            return "xx:xx";
        }

        string minutes = Math.Floor(this.LevelTime / 60).ToString("00");
        string seconds = Math.Floor(this.LevelTime % 60).ToString("00");
        return minutes + ":" + seconds;
    }

    private string getGoal(){
        return goal.ToString();
    }

    private string getCollected() {
        return collected.ToString("00");
    }

    private string getMaxCollected() {
        return "/" + maxCollected.ToString("00");
    }

    private float pixelsFromLeft(float pixels) {
        return pixels;
    }

    private float pixelsFromRight(float pixels) {
        return Screen.width - pixels;
    }

    /// <summary>
    /// Gets the current HUD
    /// </summary>
    public static HUD Instance {
        get {
            GameObject worldObject = GameObject.FindGameObjectWithTag(Tags.World);

            if (worldObject == null) {
                throw new UnityException("No World found or object with tag 'World'");
            }

            HUD hudscript = worldObject.GetComponent<HUD>();

            if (hudscript == null) {
                throw new UnityException("World object does not contain 'HUD'");
            }

            return hudscript;
        }
    }
}
