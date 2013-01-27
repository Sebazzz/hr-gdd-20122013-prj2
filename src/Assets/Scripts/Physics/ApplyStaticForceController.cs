using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Applies a small static force from the front and back of the object when another object touched the current objects trigger, except on ice
/// </summary>
internal class ApplyStaticForceController : MonoBehaviour {
    /// <summary>
    /// Defines the tags that are affected by this 
    /// </summary>
    public string AffectedTag = Tags.Sheep;

    /// <summary>
    /// Defines the force to apply on the object
    /// </summary>
    public float ForceToApply = 1f;

    void Start() {
    }

    void Update() {
       
    }

    void OnTriggerStay(Collider triggerCollider) {
        if (!this.enabled) {
            return;
        }

        if (triggerCollider.gameObject.tag != this.AffectedTag) {
            return;
        }

        Rigidbody rb = triggerCollider.gameObject.rigidbody;
        if (rb == null) {
            return;
        }

        // apply a small force
        Vector3 direction = triggerCollider.transform.position - this.transform.position;
        direction.y = 0;
        rb.AddForce(direction * this.ForceToApply, ForceMode.Force);
    }
}