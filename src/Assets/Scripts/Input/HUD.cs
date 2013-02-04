using System.Globalization;
using UnityEngine;
using System;

/// <summary>
/// Script for drawing
/// </summary>
public sealed class HUD : MonoBehaviour {
    // Skin data
    public GUISkin skin;
    public MainMenuReturnToLevelSelectBehaviour.SelectedWorld currentWorld;

    // Score data
    private Boolean minScore;
    private Boolean maxScore;
    private Boolean maxScoreTime;

    private Texture2D timeTexture;
    private Texture2D goalTexture;
    private Texture2D sheepTexture;

    private int sheepGoal = 0;
    private int collected = 0;
    private int maxCollected = 0;

    public float LevelTime { get; set; }
    public bool EnableCountDown { get; set; }
    public bool Show { get; set; }


	// Use this for initialization
	void Start () {
        if (skin == null) {
            throw(new Exception("GUISkin is needed for the HUD"));
        }

        this.Show = true;
        this.LoadTextures();
	}

    private void LoadTextures() {
        this.timeTexture = Resources.Load("Hud/hud-time") as Texture2D;
        this.goalTexture = Resources.Load("Hud/hud-goal") as Texture2D;
        this.sheepTexture = Resources.Load("Hud/hud-sheep") as Texture2D;
    }
	
	void Update () {
        if (EnableCountDown) {
            this.LevelTime -= Time.deltaTime;
        }
	}

    void OnGUI() {
        if (this.Show) {
            this.DrawHudBar();
        }

        DialogController.DrawDialogs(this.skin);
    }

    /// <summary>
    /// Sets goal
    /// </summary>
    /// <param name="goal"></param>
    public void SetGoal(int goal) {
        this.sheepGoal = goal;
    }

    /// <summary>
    /// Sets collected.
    /// </summary>
    /// <param name="amount">Is value 0 in "sheep 0/10"</param>
    public void SetNumberCollected(int amount) {
        this.collected = amount;
    }

    /// <summary>
    /// Sets maxCollected
    /// </summary>
    /// <param name="amount">Is value 10 in "sheep 0/10"</param>
    public void SetMaxCollected(int amount) {
        this.maxCollected = amount;
    }

    /// <summary>
    /// Displays A YES-NO dialog. See: http://stackoverflow.com/questions/667742/callbacks-in-c-sharp
    /// </summary>
    /// <param name="text">Text to display in dialog</param>
    /// <param name="yesAction">Action delegate to run for Yes</param>
    /// <param name="noAction">Action delegate to run for No</param>
    public void DisplayDialog(string text, Action yesAction, Action noAction) {
        GenericYesNoDialog.ShowDialog(
            String.Empty,
            text,
            new GenericYesNoDialog.ButtonInfo("Yes", yesAction),
            new GenericYesNoDialog.ButtonInfo("No", noAction));
    }

    /// <summary>
    /// Displays a score dialog.
    /// </summary>
    public void DisplayScoreDialog(bool minimalScoreReached, bool maximumScoreReached, bool timeGoalReached) {
        GameScoreDialog.ShowDialog(this.currentWorld, minimalScoreReached, maximumScoreReached, timeGoalReached);
    }

    /// <summary>
    /// Displays a death dialog.
    /// </summary>
    public void DisplayDeathDialog(String reason) {
        GameOverDialog.ShowDialog(reason);
    }
    
    private void DrawHudBar() {
        GUI.DrawTexture(new Rect(GetPixelsFromLeft(20), 20, 115, 59), timeTexture, ScaleMode.StretchToFill, true, 0);
        GUI.Label(new Rect(GetPixelsFromLeft(70), 35, 100, 40), this.GetTimeAsString(), this.LevelTime > 0 ? skin.label : skin.GetStyle("LabelRedTime"));

        GUI.DrawTexture(new Rect(GetPixelsFromLeft(155), 20, 145, 59), goalTexture, ScaleMode.StretchToFill, true, 0);
        GUI.Label(new Rect(GetPixelsFromLeft(246), 35, 10, 30), this.GetGoalAsString(), skin.GetStyle("LabelWhite"));

        GUI.DrawTexture(new Rect(GetPixelsFromLeft(320), 20, 230, 59), sheepTexture, ScaleMode.StretchToFill, true, 0);

        bool showRedLabel = this.collected >= this.sheepGoal;
        GUIStyle labelStyle;
        if (showRedLabel) {
            labelStyle = skin.GetStyle("LabelRed");
        } else {
            labelStyle = skin.GetStyle("LabelBlack");
        }

        GUI.Label(new Rect(GetPixelsFromLeft(445), 35, 50, 40), this.GetNumberCollectedAsString(), labelStyle);// x = 465 maar omdat we rechts uitlijnen is het x - width
        GUI.Label(new Rect(GetPixelsFromLeft(495), 35, 100, 40), this.GetMaxCollectedAsString(), skin.GetStyle("LabelBlack"));

        if (GUI.Button(new Rect(GetPixelsFromRight(190), 20, 55, 59), "", skin.GetStyle("RestartButton"))) {
            DisplayDialog("Would you like to restart this level?", () => AsyncSceneLoader.Load(Application.loadedLevelName), () => { });
        }

        if (GUI.Button(new Rect(GetPixelsFromRight(115), 20, 95, 59), "", skin.GetStyle("MenuButton"))) {
            DisplayDialog("Would you like to return to the menu?", () => AsyncSceneLoader.Load(Scenes.MainMenu), () => { });
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

    private string GetGoalAsString(){
        return this.sheepGoal.ToString(CultureInfo.CurrentCulture);
    }

    private string GetNumberCollectedAsString() {
        return collected.ToString("00");
    }

    private string GetMaxCollectedAsString() {
        return "/" + maxCollected.ToString("00");
    }

    private static float GetPixelsFromLeft(float pixels) {
        return pixels; // WTF Robin??? :P
    }

    private static float GetPixelsFromRight(float pixels) {
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
