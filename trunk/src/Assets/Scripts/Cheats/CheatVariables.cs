using UnityEngine;

/// <summary>
/// Defines variabeles that can be set
/// </summary>
public static class CheatVariables {
    /// <summary>
    /// Specifies if the rotation lock on the sheep <see cref="Rigidbody"/> is disabled
    /// </summary>
    [CheatVariabele("sheeprotationlock", "Lock x and z rotation of sheep. By default true.")]
    public static bool EnableSheepRotationLock = true;

    /// <summary>
    /// Specifies if the in-game cheat menu is enabled
    /// </summary>
    [CheatVariabele("gamecheats", "Show the in-game cheat button and menu.")]
    public static bool EnableInGameCheatsMenu = false;

    [CheatVariabele("supersheep", "Enlarge all sheeps in the next levels 4 times.")]
    public static bool EnableLargeSheep = false;

    [CheatVariabele("terrainbounce", "Enable a bouncy terrain.")]
    public static bool TerrainBounce = false;

    [CheatVariabele("fpscounter", "Enable a FPS counter (Frames per Second)")]
    public static bool EnableFPSCounter = false;
}