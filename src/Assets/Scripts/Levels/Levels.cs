using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Levels{
    // Create an array of levels for one country. 
    public static Level[] playCountry = { new Level("Playground") };
    //Example of another country:
    //public static Level[] anotherCountry = { new Level("Cheese"), new Level("Bacon") };

    // Add levelarray to countries to add a country
    public static Country[] countries = { new Country(playCountry) };
    
    /// <summary>
    /// Get the current level
    /// </summary>
    /// <returns>Returns Level.none in case of no new levels. (all done)</returns>
    public static Level getCurrentLevel() {
        foreach (Country country in countries) {
            if (country.hasCompleted() == false) {
                return country.getLatestLevel();
            }
        }

        return Level.NONE;
    }

    /// <summary>
    /// Get level by name
    /// </summary>
    /// <param name="name">Name of the level</param>
    /// <returns>Returns the level, throws exception if it doenst exist</returns>
    public static Level getLevelByName(String name) {
        foreach (Country country in countries) {
            Level level = country.getLevelByName(name);
            if (level != Level.NONE) {
                return level;
            }
        }

        throw new Exception("Level with name: " + name + " does not exist.");
    }
}

