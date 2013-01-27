using UnityEngine;
using System.Collections;

public class MainMenuOptionsItemBehaviour : MonoBehaviour {

	public enum OptionItems { audio, resolution, fullscreen, quality}

	public OptionItems OptionItem;
	public MainMenuOptionsBehaviour optionsBehaviour;
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {

	}

	void OnMouseUpAsButton() {
		switch (OptionItem) {
			case OptionItems.audio:
			optionsBehaviour.SetAudio();
			break;

			case OptionItems.resolution:
			optionsBehaviour.SetResolution();
			break;

			case OptionItems.fullscreen:
			optionsBehaviour.SetWindowed();
			break;

			case OptionItems.quality:
			optionsBehaviour.SetQuality();
			break;
		}
	}
}
