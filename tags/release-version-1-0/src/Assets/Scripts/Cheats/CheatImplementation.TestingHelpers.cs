using System;
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

    [CheatCommand("AttachCameraDefault", CheatCategory.TestingHelpers)]
    public static void AttachCameraToNextClickedObjectWithDefaultPosition() {
        AttachCameraToNextClickedObjectWithSpecifiedPositionAndRotation(0, 2, -7.5f, 20, 0, 0, true);
    }

    [CheatCommand("AttachCamera", CheatCategory.TestingHelpers)]
    public static void AttachCameraToNextClickedObjectWithSpecifiedPosition(float x, float y, float z, bool relativeToObject) {
        AttachCameraToNextClickedObjectWithSpecifiedPositionAndRotation(x, y, z, Single.NaN, Single.NaN, Single.NaN, relativeToObject);
    }

    [CheatCommand("AttachCameraWithRotation", CheatCategory.TestingHelpers)]
    public static void AttachCameraToNextClickedObjectWithSpecifiedPositionAndRotation(float x, float y, float z, float xRotation, float yRotation, float zRotation, bool relativeToObject) {
        Vector3 pos = new Vector3(x,y,z);
        Vector3 rot = new Vector3(xRotation, yRotation, zRotation);
        Space space = relativeToObject ? Space.Self : Space.World;

        // get camera
        Camera currentCamera = Camera.mainCamera;
        if (currentCamera == null) {
            return;
        }

        // get or add click script
        AttachCameraToClickedObjectBehaviour clickScript =
            currentCamera.gameObject.GetComponent<AttachCameraToClickedObjectBehaviour>();
        if (clickScript == null) {
            clickScript = currentCamera.gameObject.AddComponent<AttachCameraToClickedObjectBehaviour>();
        }

        clickScript.Enable(pos, rot, space);
    }
}