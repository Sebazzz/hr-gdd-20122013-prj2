using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

// Cheats implementation: Functional
public static partial class CheatImplementation {
    [CheatCommand("SetGravity", CheatCategory.JustForFun)]
    public static void SetGlobalGravityDirection(float x, float y, float z) {
        Physics.gravity = new Vector3(x, y, z);
    }


    [CheatCommand("SheepRocket", CheatCategory.JustForFun)]
    public static void LaunchAllSheepUpIntoTheAirByTheSpecifiedForce(float force) {
        GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject gameObject in sheep) {
            Rigidbody rb = gameObject.rigidbody;

            float finalForce = force*rb.mass;

            rb.AddRelativeForce(Vector3.up*finalForce, ForceMode.Impulse);
        }
    }

    [CheatCommand("DanceOfTheSheep", CheatCategory.JustForFun)]
    public static void AppliesARandomForceToEachSheep(float force) {
        GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject gameObject in sheep) {
            Rigidbody rb = gameObject.rigidbody;

            float finalForce = force*rb.mass;

            rb.AddForceAtPosition(Vector3.up*finalForce, Vector3.left + Vector3.down, ForceMode.Impulse);
        }
    }

    [CheatCommand("TakeThePill", CheatCategory.JustForFun)]
    public static void CombinesTheBestCheats() {
        SetGlobalGravityDirection(0, -4, 0);

        CheatVariables.EnableSheepRotationLock = false;
        CheatVariables.TerrainBounce = true;
        RefreshVariablesWithoutReloadingLevelMayBeUnstable();

        DiscofyTheEntireLevel();
        DisableCameraMovementBounds();
        ZoomTheCameraInOrOutByTheSpecifiedAmount(-50);
        
        StartsRainingSheepAllOverTheLevel(5, 30);
    }

    [CheatCommand("PartyMode", CheatCategory.JustForFun)]
    public static void DiscofyTheEntireLevel() {
        // start coroutine for every object
        Object[] allObjects = Object.FindObjectsOfType(typeof (Transform));

        foreach (Transform script in allObjects) {
            GameObject gameObject = script.gameObject;

            if (gameObject.name.EndsWith("_DISCO")) {
                return;
            }

            gameObject.name += "_DISCO";

            // change water material to dog
            if (gameObject.tag == Tags.Trap && gameObject.name.IndexOf("Water", StringComparison.InvariantCultureIgnoreCase) != -1) {
                MeshRenderer r = gameObject.GetComponent<MeshRenderer>();

                if (r != null) {
                    r.material = (Material)Resources.Load("Reserved", typeof(Material));
                }
            }

            // execute
            StartCoroutine(ExecuteColorChange(gameObject, null));
        }
    }

    #region Fun: Disco Sheep
    [CheatCommand("DiscoSheep", CheatCategory.JustForFun)]
    public static void ColorChangesOnEverySheep() {
        // start coroutine for every sheep
        GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject gameObject in sheep) {
            StartCoroutine(ExecuteColorChange(gameObject, "Vacht"));
        }
    }

    // ReSharper disable FunctionNeverReturns -- This is intented
    private static IEnumerator ExecuteColorChange(GameObject sheep, string filter) {
        // first find all materials
        List<Material> materials = GetMaterialsOfObject(sheep, filter);

        if (materials.Count <= 0) {
            yield break;
        }

        // start color
        {
            Color32 startColor = CreateRandomColor();

            foreach (Material material in materials) {
                material.color = startColor;
            }
        }

        // go into our endless loop of fun
        while (true) {
            // factors
            float r = Random.Range(-0.1f, 0.1f);
            float g = Random.Range(-0.1f, 0.1f);
            float b = Random.Range(-0.1f, 0.1f);

            // colorize each material
            foreach (Material material in materials) {
                Color currentColor = material.color;

                currentColor.r = ClampColorFloat(currentColor.r + r);
                currentColor.g = ClampColorFloat(currentColor.g + g);
                currentColor.b = ClampColorFloat(currentColor.b + b);

                material.color = currentColor;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    // ReSharper restore FunctionNeverReturns

    private static float ClampColorFloat(float current) {
        const float min = 0f;
        const float max = 1f;

        if (current < min) {
            return max;
        }

        if (current > max) {
            return min;
        }

        return current;
    }

    private static Color CreateRandomColor() {
        float r = Random.Range(-1f, 1f);
        float g = Random.Range(-1f, 1f);
        float b = Random.Range(-1f, 1f);
        const float a = 1f;

        return new Color(r, g, b, a);
    }

    private static List<Material> GetMaterialsOfObject(GameObject targetGameObject, string filter) {
        List<Material> materials = new List<Material>();

        Stack<GameObject> gameObjects = new Stack<GameObject>();
        gameObjects.Push(targetGameObject);

        while (gameObjects.Count > 0) {
            GameObject current = gameObjects.Pop();

            // disable emitter
            MeshRenderer renderer = current.GetComponent<MeshRenderer>();
            if ((filter == null || String.Equals(current.name, filter, StringComparison.InvariantCultureIgnoreCase)) && renderer != null) {
                materials.AddRange(renderer.materials);
            }

            // search for additional
            foreach (Transform childTransform in current.transform) {
                GameObject child = childTransform.gameObject;

                gameObjects.Push(child);
            }
        }

        return materials;
    }

    #endregion

    #region Fun: Sheep Rain
    [CheatCommand("SheepRain", CheatCategory.JustForFun)]
    public static void StartsRainingSheepAllOverTheLevel(int amountPerSecond, int timeInSeconds) {
        int reqSheep = amountPerSecond*timeInSeconds;
        if (reqSheep > 250 && !Debug.isDebugBuild) {
            CheatNotificationDialog.ShowDialog("Error", "You are not trying to crash the game, are you?");
            return;
        }

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
            for (int n = 0; n < amountPerSecondLower; n++) {
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
        var spawnPosition = new Vector3(Random.Range(lowerBounds.x, upperBounds.x),
                                        Camera.mainCamera.transform.position.y + 50f,
                                        Random.Range(lowerBounds.z, upperBounds.z));

        // determine rotation
        var rotation = new Vector3();
        rotation.y = Random.Range(0, 360);

        if (!CheatVariables.EnableSheepRotationLock) {
            rotation.x = Random.Range(0, 360);
            rotation.z = Random.Range(0, 360);
        }

        Object.Instantiate(sheepTemplate, spawnPosition, Quaternion.Euler(rotation));
    }

    #endregion

}