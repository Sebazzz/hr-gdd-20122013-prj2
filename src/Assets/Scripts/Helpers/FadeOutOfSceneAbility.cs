using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper script for fading out an object of a scene. Note: this script handles its own state and should by default be disabled.
/// </summary>
public sealed class FadeOutOfSceneAbility : MonoBehaviour {
    private const float RotationSpeed = 0.5f;
    private const float DistanceOffset = 25f;
    private const float AcceptDistance = 5f;
    private const float MaterialAlpha = 0.5f;
    private const float MaterialAlphaFadeSpeed = 0.01f;
    private static readonly Vector3 DefaultDirection = Vector3.up;

    /// <summary>
    /// Defines the movement speed for moving out of the scene
    /// </summary>
    public float MovementSpeed = 5f;

    private GameObject rootGameObject;
    private Vector3 targetPosition;
    private List<Material> materials;

    private void Update() {
        // create semi-transparant material
        if (this.materials != null) {
            foreach (Material material in materials) {
                material.color = FadeColorToAlpha(material.color);
            }
        }

        // set rotation
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - rootGameObject.transform.position);
        Quaternion currentRotation = rootGameObject.transform.rotation;
        Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * RotationSpeed);
        rootGameObject.transform.rotation = newRotation;

        // move towards until deleted
        Vector3 currentPos = this.transform.position;
        Vector3 newTarget = Vector3.Lerp(currentPos, this.targetPosition, Time.deltaTime*MovementSpeed*10);

        this.transform.position = newTarget;

        // check for deletion
        if (Vector3.Distance(newTarget, this.targetPosition) <= AcceptDistance) {
            Destroy(this.rootGameObject);
        }
    }

    private static Color FadeColorToAlpha(Color original) {
        if (original.a <= MaterialAlpha) {
            return original;
        }

        Color copy = original;
        copy.a -= MaterialAlphaFadeSpeed;

        return copy;
    }

    /// <summary>
    /// Enables the fadeing out
    /// </summary>
    public void Enable(GameObject targetGameObject) {
        this.enabled = true;

        // search for an halo in this or one of the child objects
        Behaviour halo = GetHaloRecursive(targetGameObject);

        if (halo != null) {
            halo.enabled = true;
        }

        // disable any rigid body
        Rigidbody rb = targetGameObject.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
        }

        // find a point
        float distance = Camera.mainCamera.transform.position.y + DistanceOffset;
        Vector3 target = this.transform.position + (DefaultDirection * distance);

        this.rootGameObject = targetGameObject;
        this.targetPosition = target;

        // index all materials
        this.materials = GetMaterialsOfObject(targetGameObject);

        Shader transDiffuseShader = Shader.Find("Transparent/Diffuse");
        if (transDiffuseShader == null) {
            Debug.LogWarning("Transparent/Diffuse shader could not be found. Alpha transition will not execute");
            this.materials = null;
            return;
        }

        foreach (Material material in materials) {
            material.shader = transDiffuseShader;
        }
    }

    private static List<Material> GetMaterialsOfObject(GameObject targetGameObject) {
        List<Material> materials = new List<Material>();

        Stack<GameObject> gameObjects = new Stack<GameObject>();
        gameObjects.Push(targetGameObject);

        while (gameObjects.Count > 0) {
            GameObject current = gameObjects.Pop();

            // disable emitter
            MeshRenderer renderer = current.GetComponent<MeshRenderer>();
            if (renderer != null) {
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


    private static Behaviour GetHaloRecursive(GameObject targetGameObject) {
        var gameObjects = new Stack<GameObject>();
        gameObjects.Push(targetGameObject);

        while (gameObjects.Count > 0) {
            GameObject current = gameObjects.Pop();

            // search for halo
            var halo = current.GetComponent("Halo") as Behaviour;
            if (halo != null) {
                return halo;
            }

            // try others
            foreach (Transform childTransform in current.transform) {
                gameObjects.Push(childTransform.gameObject);
            }
        }

        return null;
    }
}