using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuOptionsBehaviour : MonoBehaviour {

	public TextMesh audioMesh;
	public TextMesh resolutionMesh;
	public TextMesh windowedMesh;
	public TextMesh qualityMesh;

	private enum audioOptions { Aan, Uit }
	private Resolution[] resolutions;
	private enum windowedOptions { Aan, Uit }
	private string[] qualityOptions;

	private audioOptions audio;
	private int resolution;
	private windowedOptions windowed = 0;
	private int quality;


	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
		audio = (audioOptions) PlayerPrefs.GetInt("audio", 0);
		resolution = PlayerPrefs.GetInt("resolution", 0);
		windowed = (windowedOptions) PlayerPrefs.GetInt("windowed", 1);
		quality = PlayerPrefs.GetInt("quality", 0);

		resolutions = Screen.resolutions;
		qualityOptions = QualitySettings.names;
	}

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {

		audioMesh.text = "Audio : " + audio;
		try { resolutionMesh.text = "Resolutie : " + resolutions[resolution].width + " x " + resolutions[resolution].height; }
		catch { resolutionMesh.text = "Resolutie : " + resolutions[0].width + " x " + resolutions[0].height; }
		windowedMesh.text = "Fullscreen : " + windowed;
		qualityMesh.text = "Kwaliteit : " + qualityOptions[quality];
	
	}

	/// <summary>
	/// Write options to playerprefs
	/// </summary>
	public void SaveOptions() {
		PlayerPrefs.SetInt("audio", (int)audio);
		PlayerPrefs.SetInt("resolution", resolution);
		PlayerPrefs.SetInt("windowed", (int)windowed);
		PlayerPrefs.SetInt("Quality", quality);

		bool fullscreen;
		if(PlayerPrefs.GetInt("windowed") == 0) fullscreen = true;
		else fullscreen = false;

		Screen.SetResolution(resolutions[PlayerPrefs.GetInt("resolution")].width, resolutions[PlayerPrefs.GetInt("resolution")].height, fullscreen);

		QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", 0));
	}


	public void SetAudio() {
		audio++;

		if (audio > audioOptions.Uit) audio = 0;
	}

	public void SetResolution() {
		resolution++;

		if (resolution >= resolutions.Length) resolution = 0;
	}

	public void SetWindowed() {
		windowed++;

		if (windowed > windowedOptions.Uit) windowed = 0;
	
	}

	public void SetQuality() {
		quality++;

		if (quality >= qualityOptions.Length) quality = 0;

	}
}
