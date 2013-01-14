using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper script for fading out an object of a scene. Note: this script handles its own state and should by default be disabled.
/// </summary>
public sealed class FadeOutOfSceneAbility : MonoBehaviour {
    private const float RotationSpeed = 0.5f;
    private const float DistanceOffset = 25f;
    private const float AcceptDistance = 5f;
    private static readonly Vector3 DefaultDirection = Vector3.up;

    /// <summary>
    /// Defines the movement speed for moving out of the scene
    /// </summary>
    public float MovementSpeed = 5f;

    private GameObject rootGameObject;
    private Vector3 targetPosition;

    private void Update() {
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