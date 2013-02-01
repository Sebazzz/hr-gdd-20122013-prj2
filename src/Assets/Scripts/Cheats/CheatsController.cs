using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Helper script for processing various cheats and enable in-game cheats
/// </summary>
[RequireComponent(typeof(HUD))]
public sealed class CheatsController : MonoBehaviour {
    private static bool _HasSetEnableInGameCheatsMenuDefaultValue = false;

    private void Awake() {
        CheatInputDialog.HideDialog();

        if (!_HasSetEnableInGameCheatsMenuDefaultValue) {
            CheatVariables.EnableInGameCheatsMenu = Debug.isDebugBuild;
            _HasSetEnableInGameCheatsMenuDefaultValue = true;
        }
    }

    private void Start() {
        if (CheatVariables.EnableLargeSheep) {
            SetLargeSheep();
        }

        if (!CheatVariables.EnableSheepRotationLock) {
            RemoveSheepRigidbodyRotationConstraint();
        }

        if (CheatVariables.TerrainBounce) {
            EnableTerrainBounce();
        }
    }

    private static void EnableTerrainBounce() {
        // load material
        PhysicMaterial bounceMaterial = (PhysicMaterial)Resources.Load("BounceMaterial", typeof(PhysicMaterial));

        if (bounceMaterial == null) {
            Debug.LogError("Could not load bounce material");
        }

        // bouncy terrain
        Terrain.activeTerrain.collider.material = bounceMaterial;

        // bouncy sheep
        GameObject[] sheepArray = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        SetObjectBouncy(sheepArray, bounceMaterial);

        // bouncy dog
        GameObject[] dogArray = GameObject.FindGameObjectsWithTag(Tags.Shepherd);

        SetObjectBouncy(dogArray, bounceMaterial);
    }


    private static void SetObjectBouncy(IEnumerable<GameObject> sheepArray, PhysicMaterial bounceMaterial) {
        foreach (GameObject gameObject in sheepArray) {
            // disable physics script
            ApplyPhysicsBehaviour physicsScript = gameObject.GetComponent<ApplyPhysicsBehaviour>();

            if (physicsScript != null) {
                physicsScript.enabled = false;
            }

            // set collider material
            Collider collider = gameObject.GetComponent<Collider>();

            if (collider != null) {
                collider.material = bounceMaterial;
            }

            Rigidbody rb = gameObject.GetComponent<Rigidbody>();

            if (rb != null) {
                rb.drag = 0.15f;
            }

        }
    }

    private void Update() {
        if (CheatVariables.EnableInGameCheatsMenu && Input.GetKeyUp(KeyCode.BackQuote)) {
            CheatInputDialog.ShowDialog();
        }
    }

    private static void RemoveSheepRigidbodyRotationConstraint() {
        GameObject[] sheepArray = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject sheep in sheepArray) {
            Rigidbody rb = sheep.GetComponent<Rigidbody>();

            if (rb == null) {
                continue;
            }

            rb.constraints = RigidbodyConstraints.None;
        }
    }

    private static void SetLargeSheep() {
        const int scale = 4;

        // find all sheep and enlarge them
        GameObject[] sheepArray = GameObject.FindGameObjectsWithTag(Tags.Sheep);

        foreach (GameObject sheep in sheepArray) {
            sheep.transform.localScale = new Vector3(scale, scale, scale);

            // increase Y because collider is bigger
            var sheepCollider = sheep.GetComponent<Collider>();
            float ySize = sheepCollider.bounds.size.y;

            Vector3 pos = sheep.transform.position;
            pos.y += (ySize*scale)/2f;
            sheep.transform.position = pos;
        }
    }

    private void OnGUI() {
        if (!CheatVariables.EnableInGameCheatsMenu) {
            return;
        }

        // get the GUI style from the HUD script
        GUISkin guiStyle = this.GetComponent<HUD>().skin;

        const int width = 60;
        const int height = 30;

        int x = Screen.width - width;
        int y = Screen.height - height;

        if (GUI.Button(new Rect(x, y, width, height), "Cheats", guiStyle.GetStyle("button"))) {
            CheatInputDialog.ShowDialog();
        }

        // draw dialog
        CheatInputDialog.DrawDialog(guiStyle);
    }
}