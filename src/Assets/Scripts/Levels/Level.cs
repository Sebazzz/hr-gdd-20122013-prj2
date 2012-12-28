using UnityEngine;
using System.Collections;

public class Level {
    public static Level NONE = new Level("None");

    public static int STATE_LOCKED = 0;
    public static int STATE_UNLOCKED = 1;
    public static int STATE_DONE = 2;

    public string name = "";
    private string lockstate_key = "_lockstate";

    public Level(string name) {
        this.name = name;
    }

    /// <summary>
    /// Returns the state of this level.
    /// </summary>
    /// <returns>One of the static state variables</returns>
    public int getState() {
        return PlayerPrefs.GetInt(name + lockstate_key, STATE_LOCKED);
    }

    /// <summary>
    /// Call this once this level must be unlocked. Eg. when the previous level is set to done.
    /// </summary>
    public void unlock() {
        PlayerPrefs.SetInt(name + lockstate_key, STATE_UNLOCKED);
    }

    /// <summary>
    /// Call this when a level is done
    /// </summary>
    public void setFinished() {
        PlayerPrefs.SetInt(name + lockstate_key, STATE_DONE);
    }
}
