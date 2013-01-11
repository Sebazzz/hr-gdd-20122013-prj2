using UnityEngine;

public class Level {
    #region LevelStatus enum

    public enum LevelStatus {
        Locked = 0,
        Unlocked = 1,
        Done = 2
    }

    #endregion

    private const string SettingsLockStateKey = "_lockstate";
    public static Level None = new Level("None");

    /// <summary>
    /// Initializes the instance
    /// </summary>
    /// <param name="name"></param>
    public Level(string name) {
        this.Name = name;
    }

    public string Name { get; set; }

    /// <summary>
    /// Returns the state of this level.
    /// </summary>
    /// <returns>One of the static state variables</returns>
    public LevelStatus GetState() {
        return (LevelStatus) PlayerPrefs.GetInt(this.Name + SettingsLockStateKey, (int) LevelStatus.Locked);
    }

    /// <summary>
    /// Call this once this level must be unlocked. Eg. when the previous level is set to done.
    /// </summary>
    public void Unlock() {
        PlayerPrefs.SetInt(this.Name + SettingsLockStateKey, (int) LevelStatus.Unlocked);
    }

    /// <summary>
    /// Call this when a level is done
    /// </summary>
    public void SetFinished() {
        PlayerPrefs.SetInt(this.Name + SettingsLockStateKey, (int) LevelStatus.Done);
    }
}