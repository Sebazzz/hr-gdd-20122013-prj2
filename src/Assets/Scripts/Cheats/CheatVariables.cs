using UnityEngine;

/// <summary>
/// Defines variabeles that can be set
/// </summary>
public static class CheatVariables {
    /// <summary>
    /// Specifies if the rotation lock on the sheep <see cref="Rigidbody"/> is disabled
    /// </summary>
    [CheatVariabele("supersheep", "Enlarge all sheeps in the next levels 4 times.")]
    public static bool EnableSheepRotationLock = true;

    /// <summary>
    /// Specifies if the in-game cheat menu is enabled
    /// </summary>
    [CheatVariabele("gamecheats", "Show the in-game cheat button and menu.")]
    public static bool EnableInGameCheatsMenu = false;

    [CheatVariabele("terrainbounce", "Enable a bouncy terrain.")]
    public static bool EnableLargeSheep = false;

    [CheatVariabele("sheeprotationlock", "Lock x and z rotation of sheep. By default true.")]
    public static bool TerrainBounce = false;
}