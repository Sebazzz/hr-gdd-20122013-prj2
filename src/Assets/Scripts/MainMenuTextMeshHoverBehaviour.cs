using UnityEngine;
using System.Collections;

public class MainMenuTextMeshHoverBehaviour : MonoBehaviour {

	public TextMesh textMesh;
	private bool isHighlighted = false;


	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
		
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {

		if (isHighlighted) {
			textMesh.fontStyle = FontStyle.Bold;
		}
		else if (!isHighlighted) {
			textMesh.fontStyle = FontStyle.Normal;
		}
	}

	/// <summary>
	/// Called when hovered over collider with mouse
	/// </summary>
	void OnMouseEnter() {
		isHighlighted = true;
	}

	/// <summary>
	/// Called when mouse is no longer over collider
	/// </summary>
	void OnMouseExit() {
		isHighlighted = false;
	}
}
