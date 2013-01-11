using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Levels{
    // Create an array of Levels for one country. 
    public static Level[] PlayCountry = {
                                            new Level("Playground"),
                                            new Level("Scotland_lvl1"),
                                            new Level("Scotland_lvl2"),
                                            new Level("Scotland_lvl3"),
                                            new Level("Scotland_lvl4")
                                        };
    //Example of another country:
    //public static Level[] anotherCountry = { new Level("Cheese"), new Level("Bacon") };

    // Add levelarray to countries to add a country
    public static Country[] Countries = { new Country(PlayCountry) };
    
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
}

