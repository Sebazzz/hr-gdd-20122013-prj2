using System;
using UnityEngine;

/// <summary>
/// Represents an FPS counter
/// </summary>
/// <remarks>
/// Taken from: http://wiki.unity3d.com/index.php?title=FramesPerSecond and modified
/// </remarks>
[RequireComponent(typeof (Camera))]
public sealed class FPSCounter : MonoBehaviour {
    public float UpdateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private HUD hudComponent;
    private float timeleft; // Left time for current interval

    private float currentFPS;

    private void Start() {
        this.hudComponent = HUD.Instance;
        this.timeleft = this.UpdateInterval;
    }

    private void Update() {
        this.timeleft -= Time.deltaTime;
        this.accum += Time.timeScale/Time.deltaTime;
        ++this.frames;

        // Interval ended - update GUI text and start new interval
        if (this.timeleft <= 0.0) {
            // display two fractional digits (f2 format)
            float fps = this.accum/this.frames;
            this.currentFPS = fps;

            this.timeleft = this.UpdateInterval;
            this.accum = 0.0F;
            this.frames = 0;
        }
    }

    private void OnGUI() {
        GUISkin skin = this.hudComponent.skin;
        GUIStyle fpsStyle = skin.GetStyle("FPSCounter");

        string format = String.Format("{0:F2} FPS", this.currentFPS);

        if (this.currentFPS < 30) {
            fpsStyle.normal.textColor = Color.yellow;
        } else if (this.currentFPS < 10) {
            fpsStyle.normal.textColor = Color.red;
        } else {
            fpsStyle.normal.textColor = Color.green;
        }

        GUI.Label(
            new Rect(25, Screen.height - 50, 50, 50),
            format,
            fpsStyle);
    }
}