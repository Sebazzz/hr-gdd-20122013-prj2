using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/// <summary>
/// Cheat implementation class. These public static members are called via reflection only.
/// </summary>
public static class CheatsImplementation {
    #region Functional (useful) cheats

    [Cheat("Help")]
    public static void ShowCheatsHelpReference() {
        const string indent = "    ";

        List<string> cheatName = new List<string>();
        List<string> cheatDescription = new List<string>();

        // title for general cheats
        cheatName.Add("General commands:");
        cheatDescription.Add(null);

        // ... aggregate all general cheats and format them nicely
        IEnumerable<CheatDescriptor> cheatDescriptors = CheatReference.GetAllCheats();

        foreach (CheatDescriptor member in cheatDescriptors) {
            cheatDescription.Add(member.Description);

            // command formatted with arguments
            StringBuilder format = new StringBuilder();
            format.Append(indent + member.CommandName);

            foreach (ParameterInfo parameterInfo in  member.Parameters) {
                format.AppendFormat(" <{0}>", parameterInfo.Name);
            }

            cheatName.Add(format.ToString());
        }

        cheatName.Add(indent);
        cheatDescription.Add(indent);

        // title for enable/disable commands
        cheatName.Add("Variabeles to enable/disable:");
        cheatDescription.Add(null);

        // ... aggregate any enable/disable vars
        foreach (CheatVar<bool> cheatVar in ToggleCheatVars) {
            cheatName.Add(indent + cheatVar.Name);
            cheatDescription.Add(cheatVar.Description);
        }

        cheatName.Add("Note: Some cheats may only be applied after a level reload.");
        cheatDescription.Add(null);

        // show the dialog
        CheatReferenceDialog.ShowDialog("Cheats1337 Reference", cheatName.ToArray(), cheatDescription.ToArray(), "MonospaceLabel");
    }

    [Cheat("ClearSettings")]
    public static void ClearAllSettings() {
        PlayerPrefs.DeleteAll();

        AsyncSceneLoader.Load(Scenes.MainMenu);
    }

    [Cheat("BeTheBest")]
    public static void SetAllLevelsFullyPlayed() {
        Level currentLevel = Levels.GetFirstLevel();

        while (currentLevel != Level.None) {
            currentLevel.SetFinished();

            currentLevel = Levels.GetNextLevel(currentLevel);
        }

        AsyncSceneLoader.Load(Scenes.MainMenu);
    }

    #endregion

    #region Variabele modifications

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

        CheatNotificationDialog.ShowDialog("Error", String.Format("Variabele could not be set to '{1}': Variabele '{0}' does not exist.", variabele, value), "MonospaceLabel");
    }

    private static readonly List<CheatVar<bool>> ToggleCheatVars = new List<CheatVar<bool>>();
    private static readonly List<CheatVar<float>> FloatCheatVars = new List<CheatVar<float>>();

    static CheatsImplementation() {
        ToggleCheatVars.Add(new CheatVar<bool>("supersheep", "Enlarge all sheeps in the next levels 4 times", v => CheatsController.EnableLargeSheep = v));
        ToggleCheatVars.Add(new CheatVar<bool>("gamecheats", "Show the in-game cheat button and menu", v => CheatsController.EnableInGameCheatsMenu = v));
        ToggleCheatVars.Add(new CheatVar<bool>("terrainbounce", "Enable a bouncy terrain", v => CheatsController.TerrainBounce = v));
        ToggleCheatVars.Add(new CheatVar<bool>("sheeprotationlock", "Rotation lock of sheep. By default true.", v => CheatsController.EnableSheepRotationLock = v));
    }

    #region Nested type: CheatVar

    /// <summary>
    /// Internal helper struct for defining and setting variabeles
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private struct CheatVar<T> {
        public readonly string Name;
        public readonly Action<T> Setter;
        public readonly string Description;

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

    #endregion

    #region Unlockables

    [Cheat("PlayTheGround")]
    public static void LoadDeveloperSandboxLevel() {
        AsyncSceneLoader.Load(Scenes.Playground);
    }

    #endregion

    #region Testing Helpers

    [Cheat("EndlessCameraMovement")]
    public static void DisableCameraMovementBounds() {
        ArrowMovementCameraBehaviour cameraScript = Camera.mainCamera.GetComponent<ArrowMovementCameraBehaviour>();

        if (cameraScript != null) {
            cameraScript.BoundingBoxBottomLeft = new Vector2(-1000, -1000);
            cameraScript.BoundingBoxTopRight = new Vector2(1000, 1000);
        }
    }

    #endregion

    #region Just for Fun
        
    [Cheat("SheepRain")]
    public static void StartsRainingSheepAllOverTheLevel(int amountPerSecond, int timeInSeconds) {
        // find a sheep to clone
        GameObject sheepToClone = GameObject.FindGameObjectWithTag(Tags.Sheep);

        StartCoroutine(SheepRainExecutor(amountPerSecond, timeInSeconds, sheepToClone));
    }

    private static IEnumerator SheepRainExecutor(int amountPerSecond, int timeInSeconds, GameObject sheepTemplate) {
        float timeLeft = timeInSeconds;

        // determine terrain bounds
        Vector3 lowerBounds = Terrain.activeTerrain.transform.position;
        Vector3 upperBounds = lowerBounds + Terrain.activeTerrain.terrainData.size;

        // split in two
        int amountPerSecondLower = Mathf.FloorToInt(amountPerSecond / 2f);
        int amountPerSecondUpper = Mathf.CeilToInt(amountPerSecond / 2f);

        while (timeLeft > 0) {
            // first half of second
            for (int n=0;n<amountPerSecondLower;n++) {
                SpawnSheep(sheepTemplate, lowerBounds, upperBounds);
            }

            LevelBehaviour.Instance.RecountSheep();
            yield return new WaitForSeconds(0.5f);
            timeLeft -= 0.5f;

            // second half of second
            for (int n = 0; n < amountPerSecondUpper; n++) {
                SpawnSheep(sheepTemplate, lowerBounds, upperBounds);
            }

            LevelBehaviour.Instance.RecountSheep();
            yield return new WaitForSeconds(0.5f);
            timeLeft -= 0.5f;
        }

        yield break;
    }

    private static void SpawnSheep(GameObject sheepTemplate, Vector3 lowerBounds, Vector3 upperBounds) {
        // determine position
        Vector3 spawnPosition = new Vector3(Random.Range(lowerBounds.x, upperBounds.x),
                                            Camera.mainCamera.transform.position.y + 50f,
                                            Random.Range(lowerBounds.z, upperBounds.z));

        // determine rotation
        Vector3 rotation = new Vector3();
        rotation.y = Random.Range(0, 360);

        if (!CheatsController.EnableSheepRotationLock) {
            rotation.x = Random.Range(0, 360);
            rotation.z = Random.Range(0, 360);
        }

        Object.Instantiate(sheepTemplate, spawnPosition, Quaternion.Euler(rotation));
    }

    [Cheat("SheepRocket")]
    public static void LaunchAllSheepUpIntoTheAirByTheSpecifiedForce(float force) {
        GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject gameObject in sheep) {
            Rigidbody rb = gameObject.rigidbody;

            float finalForce = force*rb.mass;

            rb.AddRelativeForce(Vector3.up * finalForce, ForceMode.Impulse);
        }
    }

    #endregion

    #region Support methods

    private static bool StartCoroutine(IEnumerator coroutineResult) {
        CheatsController controller = AssertGetCheatsController();

        if (controller == null) {
            return false;
        }

        controller.StartCoroutine(coroutineResult);
        return true;
    }

    private static CheatsController AssertGetCheatsController() {
        GameObject worldObject = GameObject.FindGameObjectWithTag(Tags.World);

        if (worldObject != null) {
            CheatsController ctrl = worldObject.GetComponent<CheatsController>();

            if (ctrl != null) {
                return ctrl;
            }
        }

        CheatNotificationDialog.ShowDialog("Error", "You're not currently in a level and the command cannot be executed therefore.", "MonospaceLabel");
        return null;
    }

        

    #endregion

        
}