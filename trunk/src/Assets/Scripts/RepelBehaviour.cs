using System;
using UnityEngine;

public class RepelBehaviour : MonoBehaviour {
    private DogAudioController audioController;
    private bool hasBarked;

    public float OuterPower = 1500f;
    public float OuterRadius = 12f;

    public float InnerPower = 0;
    public float InnerRadius = 0;
    
    /// <summary>
    /// Specifies if force is only applied when the object is in the angle specified by <see cref="EffectAngle"/>
    /// </summary>
    public bool UseEffectAngle = false;

    /// <summary>
    /// Specifies the angle an object must be in from the center of the rotation of this object. The angle is the total angle in front of the object.
    /// For 
    /// </summary>
    public float EffectAngle = 90;

    public bool Perform2DInnerRadiusCheck = true;

    private void Awake() {
        this.audioController = this.GetComponent<DogAudioController>();
    }

    private void Update() {
        // push sheep
        Vector3 explosionPos = this.transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, this.OuterRadius);
        bool hasSeenSheep = false;
        foreach (Collider hit in colliders) {
            // check tag
            if (hit.tag != Tags.Sheep) {
                continue;
            }

            // check for drag
            if (Math.Abs(hit.rigidbody.drag - 0) <= 0.01) {
                continue;
            }

            // check for angle
            if (this.UseEffectAngle) {
                Vector3 dir = hit.transform.position - this.transform.position;

                float angle = Vector3.Angle(this.transform.forward, dir);

                //Debug.Log(String.Format("Direction: {0}; my Y: {1}; angle: {2}", dir, this.transform.eulerAngles.y, angle));

                if (angle > EffectAngle/2f) {
                    continue;
                }

            }

            // check for radius
            float force = this.InnerRadiusCheck(hit.transform) ? this.InnerPower : this.OuterPower;
            float radiusForce = this.InnerRadiusCheck(hit.transform) ? this.InnerRadius : this.OuterRadius;

            if (force < 0.1) {
                continue;
            }

            hit.rigidbody.AddExplosionForce(force, explosionPos, radiusForce, 0);
            hasSeenSheep = true;
        }

        // bark
        if (this.audioController != null) {
            if (!this.hasBarked && hasSeenSheep) {
                this.audioController.SheepProximitySound.Play();
                this.hasBarked = true;
            }

            if (this.hasBarked && !hasSeenSheep) {
                // Reset bark
                this.hasBarked = false;
            }
        }

        // debug helpers
        if (Debug.isDebugBuild && this.UseEffectAngle) {
            float splitAngle = this.EffectAngle / 2f;

            {
                Vector3 position = (this.transform.position +
                                    AngleVector3(this.transform.TransformDirection(Vector3.forward*this.OuterRadius), splitAngle));
                Debug.DrawLine(this.transform.position, position, Color.red, 0);
            }

            {
                Vector3 position = (this.transform.position +
                                    AngleVector3(this.transform.TransformDirection(Vector3.forward * this.OuterRadius), -splitAngle));
                Debug.DrawLine(this.transform.position, position, Color.red, 0);
            }
        }
    }

    /// <summary>
    /// Checks if the specified transform is in the inner radius of the current object
    /// </summary>
    /// <param name="otherObject"></param>
    /// <returns></returns>
    private bool InnerRadiusCheck(Transform otherObject) {
        if (this.InnerRadius <= 0) {
            return false;
        }

        if (this.Perform2DInnerRadiusCheck) {
            return Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z),
                                    new Vector2(otherObject.position.x, otherObject.position.z)) < this.InnerRadius;
        }

        return Vector3.Distance(this.transform.position, otherObject.position) < this.InnerRadius;
    }


    private void OnDrawGizmosSelected() {
        // draw outer radius
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(this.transform.position, this.OuterRadius);

        // draw inner radius
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(this.transform.position, this.InnerRadius);

        // draw both markers for the angle
        if (this.UseEffectAngle) {
            float splitAngle = this.EffectAngle / 2f;

            Gizmos.color = Color.red;
            {
                Vector3 position = (this.transform.position +
                                    AngleVector3(this.transform.TransformDirection(Vector3.forward * this.OuterRadius), splitAngle));
                Gizmos.DrawLine(this.transform.position, position);
            }

            {
                Vector3 position = (this.transform.position +
                                    AngleVector3(this.transform.TransformDirection(Vector3.forward * this.OuterRadius), -splitAngle));
                Gizmos.DrawLine(this.transform.position, position);
            }
        }
    }

    private static Vector3 AngleVector3(Vector3 point, float angle) {
        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        return rotation * point;
    }

}