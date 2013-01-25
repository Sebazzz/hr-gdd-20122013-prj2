using UnityEngine;
using System.Collections;
using System;

public class HUD : MonoBehaviour {

    // Skin data
    public GUISkin skin;

    public MainMenuReturnToLevelSelectBehaviour.SelectedWorld currentWorld;

    enum DialogType{
        none = 0,
        text = 1,
        score = 2,
        death = 3
    }
    private DialogType showDialog = DialogType.none;
    private Rect dialogRect;

    //Dialog data
    private string dialog_title = "";
    private string dialog_text = "";
    private string dialog_yes = "";
    private string dialog_no = "";
    private Action action_yes;
    private Action action_no;

    // Score data
    private Boolean minScore;
    private Boolean maxScore;
    private Boolean maxScoreTime;

    private Texture2D timeTexture;
    private Texture2D goalTexture;
    private Texture2D sheepTexture;

    private Texture2D timeScoreTexture;
    private Texture2D timeScoreCheckedTexture;
    private Texture2D minScoreTexture;
    private Texture2D minScoreCheckedTexture;
    private Texture2D maxScoreTexture;
    private Texture2D maxScoreCheckedTexture;

    private int goal = 0;
    private int collected = 0;
    private int maxCollected = 0;

    public float LevelTime { get; set; }

    public bool EnableCountDown { get; set; }
    public bool Show { get; set; }


	// Use this for initialization
	void Start () {
        const int width = 300;
        const int height = 200;
        dialogRect = new Rect(Screen.width / 2 - (width/2), Screen.height / 2 - (height/2), width, height);

        if (skin == null) {
            throw(new Exception("GUISkin is needed for the HUD"));
        }

        this.EnableCountDown = true;
        this.Show = true;

        loadTextures();
	}

    private void loadTextures() {
        timeTexture = Resources.Load("Hud/hud-time") as Texture2D;
        goalTexture = Resources.Load("Hud/hud-goal") as Texture2D;
        sheepTexture = Resources.Load("Hud/hud-sheep") as Texture2D;

        timeScoreTexture = Resources.Load("Hud/time") as Texture2D;
        timeScoreCheckedTexture = Resources.Load("Hud/time-checked") as Texture2D;
        minScoreTexture = Resources.Load("Hud/min_sheep") as Texture2D;
        minScoreCheckedTexture = Resources.Load("Hud/min_sheep-checked") as Texture2D;
        maxScoreTexture = Resources.Load("Hud/max_sheep") as Texture2D;
        maxScoreCheckedTexture = Resources.Load("Hud/max_sheep-checked") as Texture2D;
    }
	
	// Update is called once per frame
	void Update () {
        if (EnableCountDown) {
            this.LevelTime -= Time.deltaTime;
        }
	}

    void OnGUI() {
        if (this.Show) {
            drawTopBar();
        }

        if (showDialog == DialogType.text) {
            drawDialog();
        } else if (showDialog == DialogType.score) {
            drawScoreDialog();
        } else if (showDialog == DialogType.death) {
            drawDeathDialog();
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
        dialog_title = "";//title;
        dialog_text = text;
        action_yes = yes_action;
        action_no = no_action;
        showDialog = DialogType.text;
    }

    private void drawDialog() {
        dialogRect = GUI.Window(0, dialogRect, drawInsideDialog, dialog_title, skin.window);
    }

    private void drawInsideDialog(int dialogID) {
        GUI.Label(new Rect(25, 25, 250, 130), dialog_text, skin.GetStyle("WindowText"));

        if (GUI.Button(new Rect(30, 160, 50, 30), dialog_yes, skin.GetStyle("YesButton"))) {
            action_yes();
            showDialog = DialogType.none;
        }

        if (GUI.Button(new Rect(220, 160, 50, 30), dialog_no, skin.GetStyle("NoButton"))) {
            action_no();
            showDialog = DialogType.none;
        }
    }

    /// <summary>
    /// Displays A score dialog.
    /// </summary>
    public void DisplayScoreDialog(Boolean minScore, Boolean maxScore, Boolean maxScoreTime) {
        dialog_text = "Level done";
        this.minScore = minScore;
        this.maxScore = maxScore;
        this.maxScoreTime = maxScoreTime;
        showDialog = DialogType.score;
    }

    private void drawScoreDialog() {
        dialogRect = GUI.Window(0, dialogRect, drawInsideScoreDialog, dialog_title, skin.window);
    }

    private void drawInsideScoreDialog(int dialogID) {
        GUI.Label(new Rect(25, 25, 250, 130), dialog_text, skin.GetStyle("WindowText"));

        Texture2D minT = minScoreTexture;
        if(minScore){
            minT = minScoreCheckedTexture;
        }

        Texture2D maxT = maxScoreTexture;
        if (maxScore) {
            maxT = maxScoreCheckedTexture;
        }

        Texture2D maxTimeT = timeScoreTexture;
        if (maxScoreTime) {
            maxTimeT = timeScoreCheckedTexture;
        }


        GUI.DrawTexture(new Rect(150 - 90 - 40 , 60, 80, 80), minT, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(new Rect(150 - 40, 60, 80, 80), maxT, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(new Rect(150 + 90 - 40, 60, 80, 80), maxTimeT, ScaleMode.StretchToFill, true, 0);


        if (GUI.Button(new Rect(150 - 50 - 20, 150, 40, 40), "", skin.GetStyle("MenuScoreButton"))) {
            showDialog = DialogType.none;
            Application.LoadLevel(Scenes.MainMenu);
        }

        if (GUI.Button(new Rect(150-20, 150, 40, 40), "", skin.GetStyle("RestartScoreButton"))) {
            showDialog = DialogType.none;
            Application.LoadLevel(Application.loadedLevel);
        }

        if (GUI.Button(new Rect(150 + 50 - 20, 150, 40, 40), "", skin.GetStyle("NextScoreButton"))) {
            showDialog = DialogType.none;
            MainMenuReturnToLevelSelectBehaviour.selectedWorld = currentWorld;
            Application.LoadLevel(Scenes.MainMenu);
        }
    }

    /// <summary>
    /// Displays A death dialog.
    /// </summary>
    public void DisplayDeathDialog(String reason) {
        dialog_text = reason;
        
        showDialog = DialogType.death;
    }

    private void drawDeathDialog() {
        dialogRect = GUI.Window(0, dialogRect, drawInsideDeathDialog, "", skin.window);
    }

    private void drawInsideDeathDialog(int dialogID) {
        GUI.Label(new Rect(25, 25, 250, 130), dialog_text, skin.GetStyle("WindowText"));


        if (GUI.Button(new Rect(150 - 50, 150, 40, 40), "", skin.GetStyle("MenuScoreButton"))) {
            showDialog = DialogType.none;
            Application.LoadLevel(Scenes.MainMenu);
        }

        if (GUI.Button(new Rect(150 + 10, 150, 40, 40), "", skin.GetStyle("RestartScoreButton"))) {
            showDialog = DialogType.none;
            Application.LoadLevel(Application.loadedLevel);
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
