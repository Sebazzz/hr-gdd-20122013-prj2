using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a simple timer that keeps track of time and enables triggering on certain events
/// </summary>
public sealed class Timer {
    private float currentTime;

    public float TargetTime;

    public bool IsTriggered {
        get { return this.currentTime > TargetTime; }
    }

    public void Update() {
        this.currentTime += Time.deltaTime;
    }

    public void Reset() {
        this.currentTime = 0;
    }

    public void Trigger() {
        this.currentTime = TargetTime+1;
    }

    public Timer() {
    }

    public Timer(float targetTime) {
        this.TargetTime = targetTime;
    }
}
