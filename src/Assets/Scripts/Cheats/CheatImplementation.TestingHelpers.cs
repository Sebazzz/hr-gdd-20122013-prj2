using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Cheats implementation: Functional
public static partial class CheatImplementation {
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

    [CheatCommand("ForceEnemyWave", CheatCategory.TestingHelpers)]
    public static void ForceEnemyWavesToBeSpawned() {
        GameObject[] emitters = GameObject.FindGameObjectsWithTag(Tags.Emitter);

        foreach (GameObject emitter in emitters) {
            EmitterBehaviour emitterBehaviour = emitter.GetComponent<EmitterBehaviour>();

            if (emitterBehaviour != null && emitterBehaviour.enabled) {
                emitterBehaviour.ForceWave();
            }
        }
    }

    [CheatCommand("WorldWideSheep_Config", CheatCategory.TestingHelpers)]
    public static void StartAWebServerShowingGameScreenShotsWithConfiguration(float refreshIntervalSeconds) {
        if (RemoteInterfaceServer.IsRunning) {
            return;
        }

        Application.runInBackground = true;

        RemoteInterfaceServer.Start(refreshIntervalSeconds);
        StartCoroutine(RemoteInterfaceServer.ScreenShotPump());
    }

    [CheatCommand("WorldWideSheep", CheatCategory.TestingHelpers)]
    public static void StartAWebServerShowingGameScreenShots() {
        StartAWebServerShowingGameScreenShotsWithConfiguration(1f);
    }

    [CheatCommand("ShutdownWebClient", CheatCategory.TestingHelpers)]
    public static void ShutdownTheWebServer() {
        RemoteInterfaceServer.Shutdown();
    }
}