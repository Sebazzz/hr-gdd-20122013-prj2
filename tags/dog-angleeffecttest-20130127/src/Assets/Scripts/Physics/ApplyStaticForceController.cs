using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Applies a small static force from the front and back of the object when another object touched the current object, except on ice
/// </summary>
internal class ApplyStaticForceController : MonoBehaviour {
    private List<FollowObject> objects;

    /// <summary>
    /// Defines the tags that are affected by this 
    /// </summary>
    public string AffectedTag = Tags.Sheep;

    /// <summary>
    /// Defines the force to apply on the object
    /// </summary>
    public float ForceToApply = 1f;

    void Start() {
        this.objects = new List<FollowObject>();
    }

    void Update() {
        List<FollowObject> objectsToRemove = new List<FollowObject>(1);

        foreach (FollowObject followObject in objects) {
            if (followObject.TimeLeft-- < 0) {
                objectsToRemove.Add(followObject);
                return;
            }

            followObject.Body.AddRelativeForce(Vector3.back * this.ForceToApply, ForceMode.VelocityChange);
        }

        foreach (FollowObject followObject in objectsToRemove) {
            this.objects.Remove(followObject);
        }
    }

    void OnCollisionEnter(Collision collisionInfo) {
        if (!this.enabled) {
            return;
        }

        if (collisionInfo.gameObject.tag != this.AffectedTag) {
            return;
        }

        Rigidbody rb = collisionInfo.rigidbody;
        if (rb == null) {
            return;
        }

        // check if object is in front or back
        float dotProduct = Vector3.Dot(this.transform.position, collisionInfo.gameObject.transform.position);

        bool isFront;
        if (dotProduct > 0.5) {
            isFront = true;
        } else if (dotProduct < -0.5) {
            isFront = false;
        } else {
            return;
        }

        // ... calculate the direction to apply force to
        Vector3 point;
        if (isFront) {
            point = this.transform.TransformDirection(Vector3.forward);
        } else {
            point = this.transform.TransformDirection(Vector3.back);
        }

        Vector3 relativeForceDirection = collisionInfo.gameObject.transform.InverseTransformDirection(point);

        Debug.Log(String.Format("Dot:{0}; Point: {1}; Relative force dir: {2}", dotProduct, point, relativeForceDirection));

        // apply a small force
        //rb.AddRelativeForce(Vector3.back * this.ForceToApply, ForceMode.VelocityChange);
        this.objects.Add(new FollowObject(collisionInfo.gameObject, rb, 1));
    }

    private class FollowObject {
        public GameObject GObject;
        public Rigidbody Body;
        public float TimeLeft;

        public FollowObject(GameObject gObject, Rigidbody body, float timeLeft) {
            this.GObject = gObject;
            this.Body = body;
            this.TimeLeft = timeLeft;
        }
    }
}