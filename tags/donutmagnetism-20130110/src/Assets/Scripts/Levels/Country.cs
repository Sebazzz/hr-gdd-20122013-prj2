using System.Collections;

public class Country {
    public Level[] levels;

    public Country(Level[] levels) {
        this.levels = levels;
    }

    /// <summary>
    /// Has this country been completed
    /// </summary>
    /// <returns>True if completed, false if not</returns>
    public bool hasCompleted() {
        return (levels[levels.Length].getState() == Level.STATE_UNLOCKED);
    }

    /// <summary>
    /// Gets the only unlocked level in the array. 
    /// </summary>
    /// <returns>Only unlocked level, Level.none if there are none</returns>
    public Level getLatestLevel(){
        foreach (Level level in levels) {
            if (level.getState() == Level.STATE_UNLOCKED) {
                return level;
            }
        }

        return Level.NONE;
    }

    /// <summary>
    /// Get level by name
    /// </summary>
    /// <param name="name">Name of the level</param>
    /// <returns>Returns the level, returns Level.none if it doesnt exist</returns>
    public Level getLevelByName(string name) {
        foreach (Level level in levels) {
            if (level.name == name) {
                return level;
            }
        }

        return Level.NONE;
    }

}

