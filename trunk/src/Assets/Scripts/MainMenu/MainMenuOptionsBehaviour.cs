using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuOptionsBehaviour : MonoBehaviour {

	public TextMesh audioMesh;
	public TextMesh resolutionMesh;
	public TextMesh windowedMesh;
	public TextMesh qualityMesh;

	private enum audioOptions { On, Off }
	private Resolution[] resolutions;
	private enum windowedOptions { On, Off }
	private string[] qualityOptions;

	private audioOptions audioOption;
	private int resolution;
	private windowedOptions windowed = 0;
	private int quality;


	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	    bool firstTime = PlayerPrefs.GetInt("audio", 0) == 0 && PlayerPrefs.GetInt("resolution", 0) == 0 && (windowedOptions)PlayerPrefs.GetInt("windowed", 0) == 0 && PlayerPrefs.GetInt("quality", 0) == 0;

	    audioOption = (audioOptions) PlayerPrefs.GetInt("audio", 0);
		resolution = PlayerPrefs.GetInt("resolution", 0);
		windowed = (windowedOptions) PlayerPrefs.GetInt("windowed", 0);
		quality = PlayerPrefs.GetInt("quality", 0);

		resolutions = Screen.resolutions;
		qualityOptions = QualitySettings.names;

		if (firstTime) { SaveOptions(); return; }

		bool fullscreen;
		if (PlayerPrefs.GetInt("windowed", 0) == 0) fullscreen = true;
		else fullscreen = false;

        if (Screen.GetResolution.Length > resolution) {
            if ((Screen.GetResolution[0].height != Screen.resolutions[resolution].height) &&
                (Screen.GetResolution[0].width != Screen.resolutions[resolution].width))
                Screen.SetResolution(resolutions[PlayerPrefs.GetInt("resolution")].width,
                                     resolutions[PlayerPrefs.GetInt("resolution")].height, fullscreen);
        }

	    if(QualitySettings.GetQualityLevel() != quality)
			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality", 0));
	}

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {

		audioMesh.text = "Audio : " + audioOption;
		try { resolutionMesh.text = "Resolution : " + resolutions[resolution].width + " x " + resolutions[resolution].height; }
		catch { resolutionMesh.text = "Resolution : " + resolutions[0].width + " x " + resolutions[0].height; }
		windowedMesh.text = "Fullscreen : " + windowed;
		qualityMesh.text = "Quality : " + qualityOptions[quality];
	
	}

	/// <summary>
	/// Write options to playerprefs
	/// </summary>
	public void SaveOptions() {
		PlayerPrefs.SetInt("resolution", resolution);
		PlayerPrefs.SetInt("windowed", (int)windowed);
		//PlayerPrefs.SetInt("Quality", quality);

		bool fullscreen;
		if(PlayerPrefs.GetInt("windowed") == 0) fullscreen = true;
		else fullscreen = false;

		Debug.Log("fullscreen " + fullscreen);

	    int res = PlayerPrefs.GetInt("resolution");
        if (resolutions.Length > res) {
            if (Screen.height != resolutions[res].height && Screen.width != resolutions[res].width || fullscreen != Screen.fullScreen)
                Screen.SetResolution(resolutions[PlayerPrefs.GetInt("resolution")].width,
                                     resolutions[PlayerPrefs.GetInt("resolution")].height, fullscreen);
        }

	    //QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", 0));
	}


	public void SetAudio() {
		audioOption++;

		if (audioOption > audioOptions.Off) audioOption = 0;

		PlayerPrefs.SetInt("audio", (int)audioOption);
	}

	public void SetResolution() {
		resolution++;

		if (resolution >= resolutions.Length) resolution = 0;
	}

	public void SetWindowed() {
		windowed++;

		if (windowed > windowedOptions.Off) windowed = 0;
	
	}

	public void SetQuality() {
		quality++;

		if (quality >= qualityOptions.Length) quality = 0;

		PlayerPrefs.SetInt("Quality", quality);
		QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", 0));

	}
}
