using UnityEngine;
using System.Collections;
using System;

public class CursorController : MonoBehaviour {
    public Texture cursorImage;
    public Vector2 offset;

	// Use this for initialization
	void Start () {
	    Screen.showCursor = false;

        if (cursorImage == null) {
            throw new Exception("A cursor image is needed.");
        }
	}
	
	// Update is called once per frame
	void OnGUI () {
	    Vector3 mousePos = Input.mousePosition;
        Rect position = new Rect(mousePos.x + offset.x, (Screen.height - mousePos.y) + offset.y, cursorImage.width, cursorImage.height);
        GUI.Label(position, cursorImage);
	}
}