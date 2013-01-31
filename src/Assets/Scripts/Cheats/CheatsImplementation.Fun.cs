using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cheats implementation: Functional
public static partial class CheatsImplementation {
    [Cheat("SetGravity")]
    public static void SetGlobalGravityDirection(float x, float y, float z) {
        Physics.gravity = new Vector3(x, y, z);
    }


    [Cheat("SheepRocket")]
    public static void LaunchAllSheepUpIntoTheAirByTheSpecifiedForce(float force) {
        GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject gameObject in sheep) {
            Rigidbody rb = gameObject.rigidbody;

            float finalForce = force*rb.mass;

            rb.AddRelativeForce(Vector3.up*finalForce, ForceMode.Impulse);
        }
    }

    [Cheat("DanceOfTheSheep")]
    public static void AppliesARandomForceToEachSheep(float force) {
        GameObject[] sheep = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject gameObject in sheep) {
            Rigidbody rb = gameObject.rigidbody;

            float finalForce = force*rb.mass;

            rb.AddForceAtPosition(Vector3.up*finalForce, Vector3.left + Vector3.down, ForceMode.Impulse);
        }
    }

    #region Fun: Sheep control

    [Cheat("ControllableSheep")]
    public static void EnablesSheepToBeControlledByArrowKeysOptionallyDisablingControlHelperEffects(bool disableControlEffects) {
        // find dog marker
        GameObject sourceDog = GameObject.FindGameObjectWithTag(Tags.Shepherd);
        GameObject selectionMarker = null;

        if (sourceDog != null) {
            // get the projector
            foreach (Transform tr in sourceDog.transform) {
                if (tr.gameObject.name == "SelectionProjector") {
                    selectionMarker = tr.gameObject;
                    break;
                }
            }
        }


        // disable dog effects
        GameObject[] dogs = GameObject.FindGameObjectsWithTag(Tags.Shepherd);

        foreach (GameObject dog in dogs) {
            RepelBehaviour repeller = dog.GetComponent<RepelBehaviour>();

            if (repeller != null) {
                repeller.enabled = !disableControlEffects;
            }
        }

        // attach the controlling to the sheep
        GameObject[] sheepArr = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject sheep in sheepArr) {
            CheatControlSheepByArrowKeysBehaviour c = sheep.AddComponent<CheatControlSheepByArrowKeysBehaviour>();
            c.SetMarker(selectionMarker);

            MagneticBehaviour magnet = sheep.GetComponent<MagneticBehaviour>();
            if (magnet != null) {
                magnet.enabled = !disableControlEffects;
            }
        }

        CheatNotificationDialog.ShowDialog("Instruction", "Control the sheep using the JIKL keys (similar to WSAD). Select an sheep using the middle mouse button, unselect it by clicking again.", "MonospaceLabel");
    }

    #endregion

    #region Fun: Sheep Rain
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

        if (!CheatsController.EnableSheepRotationLock) {
            rotation.x = Random.Range(0, 360);
            rotation.z = Random.Range(0, 360);
        }

        Object.Instantiate(sheepTemplate, spawnPosition, Quaternion.Euler(rotation));
    }

    #endregion

}