using System;
using UnityEngine;

public class RepelBehaviour : MonoBehaviour {
    private DogAudioController audioController;

    private bool hasBarked;
    public float power = 50.0F;
    public float radius = 10.0F;

    public float InnerPower = 0;
    public float InnerRadius = 0;

    public bool Perform2DInnerRadiusCheck = true;

    private void Awake() {
        this.audioController = this.GetComponent<DogAudioController>();
    }

    private void Update() {
        Vector3 explosionPos = this.transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, this.radius);

        bool hasSeenSheep = false;
        foreach (Collider hit in colliders) {
            float force = this.InnerRadiusCheck(hit.transform) ? this.InnerPower : this.power;
            float radiusForce = this.InnerRadiusCheck(hit.transform) ? this.InnerRadius : this.radius;

            if (force < 0.1) {
                continue;
            }

            if (hit.tag == Tags.Sheep && Math.Abs(hit.rigidbody.drag - 0) > 0.01) {
                hit.rigidbody.AddExplosionForce(force, explosionPos, radiusForce, 0);
                hasSeenSheep = true;
            }
        }

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
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(this.transform.position, this.radius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(this.transform.position, this.InnerRadius);
    }
}