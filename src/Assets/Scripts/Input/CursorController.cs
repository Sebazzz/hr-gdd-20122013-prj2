using UnityEngine;
using System.Collections;
using System;

public class CursorController : MonoBehaviour {
    public Texture2D cursorImage;
    public Vector2 offset;

	// Use this for initialization
	void Start () {

        if (cursorImage == null) {
            throw new Exception("A cursor image is needed.");
        }

        if (cursorImage.height != 32 || cursorImage.width != 32) {
            throw new Exception("The cursor image needs to be 32x32");
        }
	}

    void OnMouseEnter() {
        Cursor.SetCursor(cursorImage, offset, CursorMode.Auto);
    }

    void OnMouseExit() {
        Cursor.SetCursor(cursorImage, offset, CursorMode.Auto);
    }
}