using System.Collections.Generic;
using UnityEngine;
using System;

public class Level {
    #region LevelStatus enum

    public enum LevelStatus {
        Locked = 0,
        Unlocked = 1,
        Done = 2
    }

    #endregion

    [Flags]
    public enum Score {
        None = 0,
        MinSheep = 1,
        MaxSheep = 2,
        MaxSheepWithinTime = 4
    }

    private const string SettingsLockStateKey = "_lockstate";
    private const string SettingsScoresKey = "_scores";
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
    /// Adds one of the score flags
    /// </summary>
    /// <param name="score">Flag</param>
    public void AddScoreFlag(Score score) {
        Score s = GetScore();
        s |= score;
        PlayerPrefs.SetInt(this.Name + SettingsScoresKey, (int)s);
    }

    public Boolean HasFlag(Score flag) {
        return ((GetScore() & flag) != 0);
    }

    /// <summary>
    /// Gets the full score. For simple tests, use HasFlag for boolean
    /// </summary>
    /// <returns>Score</returns>
    public Score GetScore() {
        return (Score)PlayerPrefs.GetInt(this.Name + SettingsScoresKey, (int)Score.None);
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

    protected bool Equals(Level other) {
        return string.Equals(this.Name, other.Name);
    }

    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj)) {
            return false;
        }
        if (ReferenceEquals(this, obj)) {
            return true;
        }
        if (obj.GetType() != this.GetType()) {
            return false;
        }
        return Equals((Level) obj);
    }

    public override int GetHashCode() {
        return (this.Name != null ? this.Name.GetHashCode() : 0);
    }

    public static bool operator ==(Level left, Level right) {
        return Equals(left, right);
    }

    public static bool operator !=(Level left, Level right) {
        return !Equals(left, right);
    }
}