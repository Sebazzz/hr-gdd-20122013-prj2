using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

// Cheats implementation: Functional
public static partial class CheatImplementation {
    [CheatCommand("Help", CheatCategory.GeneralCommands)]
    public static void ShowCheatsHelpReference() {
        // show the dialog
        CheatReferenceDialog.ShowDialog("Cheats Reference", 
                                         CheatService.GetAllCommandsByHumanReadableCategory(),
                                         CheatService.GetAllVariabeles(),
                                        "CheatDialogLabel");
    }

    [CheatCommand("RepeatCommand", CheatCategory.GeneralCommands)]
    public static void RepeatSpecifiedCommand(float timeDelay, int numberOfTimes, string command) {
        StartCoroutine(RepeatSpecifiedCommandExecutor(timeDelay, numberOfTimes, command));
    }

    private static IEnumerator RepeatSpecifiedCommandExecutor(float timeDelay, int numberOfTimes, string command) {
        while (numberOfTimes-- > 0) {
            CheatService.ExecuteRawCommand(command, false);

            yield return new WaitForSeconds(timeDelay);
        }

        yield break;
    }

    [CheatCommand("ClearSettings", CheatCategory.TestingHelpers)]
    public static void ClearAllSettings() {
        PlayerPrefs.DeleteAll();

        AsyncSceneLoader.Load(Scenes.MainMenu);
    }

    [CheatCommand("BeTheBest", CheatCategory.TestingHelpers)]
    public static void SetAllLevelsFullyPlayed() {
        Level currentLevel = Levels.GetFirstLevel();

        while (currentLevel != Level.None) {
            currentLevel.SetFinished();

            currentLevel = Levels.GetNextLevel(currentLevel);
        }

        AsyncSceneLoader.Load(Scenes.MainMenu);
    }

    [CheatCommand("Reload", CheatCategory.TestingHelpers)]
    public static void ReloadTheCurrentLevel() {
        AsyncSceneLoader.Load(Application.loadedLevelName);
    }
}