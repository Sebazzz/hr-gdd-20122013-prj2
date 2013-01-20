using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Levels{
    // Create an array of Levels for one country. 
    public static Level[] Scotland = {
                                            new Level("Playground"),
                                            new Level("Scotland_lvl1"),
                                            new Level("Scotland_lvl2"),
                                            new Level("Scotland_lvl3"),
                                            new Level("Scotland_lvl4")
                                        };

	public static Level[] Holland = {
											new Level("Holland_lvl1"),
											new Level("Holland_lvl2"),
											new Level("Holland_lvl3"),
											new Level("Holland_lvl4")
										};

	public static Level[] Canada = {
										   new Level("Level1"),
										   new Level("Level2"),
										   new Level("Level3"),
										   new Level("Level4")
										};

	public static Level[] Australia = {
											new Level("Australia_lvl1"),
											new Level("Australia_lvl2"),
											new Level("Australia_lvl3"),
											new Level("Australia_lvl4")
										};
    //Example of another country:
    //public static Level[] anotherCountry = { new Level("Cheese"), new Level("Bacon") };

    // Add levelarray to countries to add a country
    public static Country[] Countries = { new Country(Scotland), new Country(Holland), new Country(Canada), new Country(Australia) };
    
    /// <summary>
    /// Get the current level
    /// </summary>
    /// <returns>Returns Level.none in case of no new Levels. (all done)</returns>
    public static Level GetCurrentLevel() {
        foreach (Country country in Countries) {
            if (country.HasBeenCompleted() == false) {
                return country.GetLatestLevel();
            }
        }

        return Level.None;
    }

    /// <summary>
    /// Get level by name
    /// </summary>
    /// <param name="name">Name of the level</param>
    /// <returns>Returns the level, throws exception if it doenst exist</returns>
    public static Level GetLevelByName(String name) {
        foreach (Country country in Countries) {
            Level level = country.GetLevelByName(name);
            if (level != Level.None) {
                return level;
            }
        }

        throw new Exception("Level with name: " + name + " does not exist.");
    }

    /// <summary>
    /// Gets the level after the level that has been supplied
    /// </summary>
    /// <param name="current">The level from which the next one should be found</param>
    /// <returns>The next level. Smaakt naar gravel. Will return Level.none if there is no next level.</returns>
    public static Level GetNextLevel(Level current) {
        Boolean foundCurrent = false;
        for (int countryIterator = 0; countryIterator < Countries.Length; countryIterator++) {
            for (int levelIterator = 0; levelIterator < Countries[countryIterator].Levels.Length; levelIterator++) {
                // Gets the current level from the iteration
                Level level = Countries[countryIterator].Levels[levelIterator];

                // If foundCurrent is true we found it one iteration ago, and we can safely return this one
                if (foundCurrent) {
                    return level;
                }

                // If the current one is the one we are looking for, set foundCurrent
                if (current == level) {
                    foundCurrent = true;
                }
            }
        }

        // If foundCurrent is true, but we came here, we reached the end and the player won the last level.
        if (foundCurrent) {
            return Level.None;
        }

        throw new Exception("Level does not exist.");
    }
}

