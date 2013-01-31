using System;
using System.Collections.Generic;
using UnityEngine;

// Cheats implementation: Functional
public static partial class CheatsImplementation {
    private static readonly List<CheatVar<bool>> ToggleCheatVars = new List<CheatVar<bool>>();
    //private static readonly List<CheatVar<float>> FloatCheatVars = new List<CheatVar<float>>();

    static CheatsImplementation() {
        ToggleCheatVars.Add(new CheatVar<bool>("supersheep", "Enlarge all sheeps in the next levels 4 times",
                                               v => CheatsController.EnableLargeSheep = v));
        ToggleCheatVars.Add(new CheatVar<bool>("gamecheats", "Show the in-game cheat button and menu",
                                               v => CheatsController.EnableInGameCheatsMenu = v));
        ToggleCheatVars.Add(new CheatVar<bool>("terrainbounce", "Enable a bouncy terrain",
                                               v => CheatsController.TerrainBounce = v));
        ToggleCheatVars.Add(new CheatVar<bool>("sheeprotationlock", "Rotation lock of sheep. By default true.",
                                               v => CheatsController.EnableSheepRotationLock = v));
    }

    [Cheat("enable")]
    public static void EnableTheSpecifiedVariabele(string variabele) {
        SetBooleanVariabeleValue(variabele, true);
    }

    [Cheat("disable")]
    public static void DisableTheSpecifiedVariabele(string variabele) {
        SetBooleanVariabeleValue(variabele, false);
    }

    private static void SetBooleanVariabeleValue(string variabele, bool value) {
        // select member of cheat controller
        foreach (var cheatVar in ToggleCheatVars) {
            if (String.Equals(cheatVar.Name, variabele, StringComparison.InvariantCultureIgnoreCase)) {
                cheatVar.Setter.Invoke(value);

                Debug.Log(cheatVar.Name + ": " + value);
                return;
            }
        }

        CheatNotificationDialog.ShowDialog("Error",
                                           String.Format(
                                               "Variabele could not be set to '{1}': Variabele '{0}' does not exist.",
                                               variabele, value));
    }

    #region Nested type: CheatVar

    /// <summary>
    /// Internal helper struct for defining and setting variabeles
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private struct CheatVar<T> {
        public readonly string Description;
        public readonly string Name;
        public readonly Action<T> Setter;

        public CheatVar(string name, Action<T> setter) {
            this.Name = name;
            this.Setter = setter;
            this.Description = "No description";
        }

        public CheatVar(string name, string description, Action<T> setter) {
            this.Name = name;
            this.Description = description;
            this.Setter = setter;
        }
    }

    #endregion
}