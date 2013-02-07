/// <summary>
/// Represents a country that can be unlocked
/// </summary>
public class Country {
    /// <summary>
    /// Initializes this instance
    /// </summary>
    /// <param name="levels"></param>
    public Country(Level[] levels) {
        this.Levels = levels;
    }

    /// <summary>
    /// Gets or sets the levels associated to this country
    /// </summary>
    public Level[] Levels { get; set; }

    /// <summary>
    /// Has this country been completed
    /// </summary>
    /// <returns>True if completed, false if not</returns>
    public bool HasBeenCompleted() {
        return (this.Levels[this.Levels.Length-1].GetState() == Level.LevelStatus.Done);
    }

    /// <summary>
    /// Gets the only unlocked level in the array. 
    /// </summary>
    /// <returns>Only unlocked level, Level.none if there are none</returns>
    public Level GetLatestLevel() {
        foreach (Level level in this.Levels) {
            if (level.GetState() == Level.LevelStatus.Unlocked) {
                return level;
            }
        }

        return Level.None;
    }

    /// <summary>
    /// Get level by name
    /// </summary>
    /// <param name="name">Name of the level</param>
    /// <returns>Returns the level, returns Level.none if it doesnt exist</returns>
    public Level GetLevelByName(string name) {
        foreach (Level level in this.Levels) {
            if (level.Name == name) {
                return level;
            }
        }

        return Level.None;
    }
}